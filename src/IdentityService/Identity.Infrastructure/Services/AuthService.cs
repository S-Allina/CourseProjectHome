using AutoMapper;
using FluentValidation;
using Identity.Application.Dto;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Identity.Domain.Enums;
using Identity.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Identity.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IValidator<ResetPasswordDto> _validator;
        private const string purpose = "ResetPassword";
        private const string separator = ", ";

        public AuthService(IEmailService emailService, ICurrentUserService currentUserService,
            UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserService> logger, IValidator<ResetPasswordDto> validator)
        {
            _emailService = emailService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
        }

        public async Task<UserDto> LoginAsync(UserLoginRequestDto request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var user = await AuthenticateUserAsync(request);

            await CheckLockoutAsync(user);

            var userResponse = _mapper.Map<UserDto>(user);

            userResponse.UpdateAt = DateTime.Now;

            return userResponse;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) throw new Exception($"Account with email {email} not found");

            if (user.Status == Statuses.Unverify) throw new Exception($"Email {email} not verify");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendPasswordResetEmailAsync(user, token);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null) throw new Exception("Account not found.");

            await ResetUserPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            return true;
        }

        private async Task ResetUserPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            var isValidToken = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, purpose, token);

            if (!isValidToken) throw new Exception("Invalid token");

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded) throw new Exception($"Failed to reset password: {string.Join(separator, result.Errors)}");
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

            if (user == null)
                throw new Exception("User not found.");

            var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isValidPassword)
            {
                await HandleAuthenticationFailureAsync(user);
                throw new Exception("Invalid email or password. Please try again.");
            }

            return user;
        }
    }
}
