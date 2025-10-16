using AutoMapper;
using FluentValidation;
using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Identity.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<UserService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidator<ResetPasswordDto> _validator;
        private const string purpose = "ResetPassword";
        private const string separator = ", ";

        public AuthService(ITokenService tokenService, IEmailService emailService, ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserService> logger, SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor, IValidator<ResetPasswordDto> validator)
        {
            _emailService = emailService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
            _logger = logger;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _validator = validator;
        }

        public async Task<CurrentUserDto> LoginAsync(UserLoginRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await AuthenticateUserAsync(request);

            await CheckLockoutAsync(user);

            var (accessToken, refreshToken) = await GenerateTokensAsync(user);

            await UpdateUserRefreshTokenAsync(user, refreshToken);

            var userResponse = _mapper.Map<CurrentUserDto>(user);

            userResponse.AccessToken = accessToken;
            userResponse.RefreshToken = refreshToken;
            userResponse.UpdateAt = DateTime.Now;

            return userResponse;
        }

        private async Task<CurrentUserDto> LoginWithoutPasswordAsync(ApplicationUser user)
        {
            var (accessToken, refreshToken) = await GenerateTokensAsync(user);

            await UpdateUserRefreshTokenAsync(user, refreshToken);

            var userResponse = _mapper.Map<CurrentUserDto>(user);

            userResponse.AccessToken = accessToken;
            userResponse.RefreshToken = refreshToken;
            userResponse.UpdateAt = DateTime.Now;

            return userResponse;
        }

        public async Task<CurrentUserDto> GoogleCallBackAsync(string returnUrl, string remoteError)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                throw new Exception($"Error from Google: {remoteError}");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new Exception("External login info is null.");
            }

            var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new Exception("Could not retrieve email from Google.");
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = user.LastName +" "+ user.FirstName,
                    Email = userEmail,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    throw new Exception($"Failed to create user: {string.Join(separator, createResult.Errors)}");
                }
            }
            var currentUser = await LoginWithoutPasswordAsync(user);

            return currentUser;
        }

        public async Task<CurrentUserDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            _logger.LogInformation("Refresh token");
            var refreshTokenHash = HashRefreshToken(request.RefreshToken);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenHash);

            if (user == null)
            {
                _logger.LogError("Invalid refresh token");
                throw new Exception("Invalid refresh token");
            }

            ValidateRefreshToken(user);

            var newAccessToken = await _tokenService.GenerateTokenAsync(user);
            var newRefreshTokenHash = HashRefreshToken(user.RefreshToken);

            _logger.LogInformation("Access token generated successful.");
            var currentUser = _mapper.Map<CurrentUserDto>(user);

            currentUser.AccessToken = newAccessToken;
            currentUser.RefreshToken = newRefreshTokenHash;
            currentUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

            return currentUser;
        }

        public async Task<RevokeTokenResponseDto> RevokeRefreshTokenAsync(RevokeTokenRequestDto request)
        {
            _logger.LogInformation("revoking refresh token.");

            var refreshTokenHash = HashRefreshToken(request.Token);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshTokenHash);

            if (user == null)
            {
                _logger.LogError("Invalid refresh token");
                throw new Exception("Invalid refresh token");
            }

            ValidateRefreshToken(user);

            await RevokeRefreshTokenAsync(user);

            return new RevokeTokenResponseDto { Message = "Refresh token revoked successfully." };
        }

        public async Task ForgotPasswordAsync()
        {
            var userId = _currentUserService.GetUserId();

            var userDto = await _userManager.FindByIdAsync(userId);

            var user = _mapper.Map<ApplicationUser>(userDto);

            var token = await _tokenService.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendPasswordResetEmailAsync(user, token);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var isValidRequest = _validator.Validate(resetPasswordDto);

            if (!isValidRequest.IsValid)
            {
                string errorMessages = string.Empty;
;               isValidRequest.Errors.Select(e=>errorMessages+ separator + e.ErrorMessage);
                throw new IdentityException(errorMessages);
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            await ResetUserPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            return true;
        }
        
        private async Task ResetUserPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var dectoken = WebUtility.UrlDecode(token);

            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, purpose, dectoken);
            if (!isValidToken)
            {
                _logger.LogError("Invalid token for password reset for user: {Email}", user.Email);
                throw new Exception("Invalid token");
            }

            var result = await _userManager.ResetPasswordAsync(user, dectoken, newPassword);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to reset password for user: {Email}", user.Email);
                throw new Exception($"Failed to reset password: {string.Join(separator, result.Errors)}");
            }
        }

        private string HashRefreshToken(string refreshToken)
        {
            using var sha256 = SHA256.Create();

            var refreshTokenHashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));

            return Convert.ToBase64String(refreshTokenHashBytes);
        }

        private void ValidateRefreshToken(ApplicationUser user)
        {
            if (user.RefreshTokenExpiryTime < DateTime.Now)
            {
                _logger.LogError("Refresh token expired.");
                throw new Exception("Refresh token expired.");
            }
        }

        private async Task CheckLockoutAsync(ApplicationUser user)
        {
            var isLockedOut = await _userManager.IsLockedOutAsync(user);
            if (isLockedOut)
            {
                _logger.LogWarning("User is locked out: {Email}", user.Email);
                throw new Exception("The user is locked out, please try again later or try resetting the password.");
            }
        }

        private async Task HandleAuthenticationFailureAsync(ApplicationUser user)
        {
            if (user != null)
            {
                await _userManager.AccessFailedAsync(user);

                await CheckLockoutAsync(user);
            }
        }

        private async Task<ApplicationUser> AuthenticateUserAsync(UserLoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (user == null || !isValidPassword)
            {
                await HandleAuthenticationFailureAsync(user);
                throw new Exception("Invalid email or password. Please try again.");
            }

            return user;
        }

        private async Task RevokeRefreshTokenAsync(ApplicationUser user)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(separator, result.Errors.Select(e => e.Description));

                _logger.LogError("Failed to update user : {errors}", errors);
                throw new Exception($"Failed to revoke refresh token: {errors}");
            }
        }

        private async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(ApplicationUser user)
        {
            var accessToken = await _tokenService.GenerateTokenAsync(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return (accessToken, refreshToken);
        }

        private async Task UpdateUserRefreshTokenAsync(ApplicationUser user, string refreshToken)
        {
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));

            user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(2);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(separator, result.Errors.Select(e => e.Description));

                _logger.LogError("Failed to update user : {errors}", errors);
                throw new Exception($"Failed to update user : {errors}");
            }
        }
    }
}
