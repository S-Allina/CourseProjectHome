using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cors;
using Identity.Domain.Entity;
using Identity.Application.Interfaces;
using Identity.Application.Dto;
using Users.Application.Dto;

namespace Identity.Presentation.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IAuthService _authService;
        private readonly string _frontUrl;
        private ResponseDto response;

        public AuthController(SignInManager<ApplicationUser> signInManager, IUserRegistrationService userRegistrationService, IAuthService authService, 
            IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userRegistrationService = userRegistrationService;
            _authService = authService;
            _frontUrl = configuration["FrontUrl"];
            response = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<ResponseDto> Register([FromBody] UserRegistrationRequestDto request)
        {
            response.Result = await _userRegistrationService.RegisterAsync(request);

            return response;
        }

        [HttpPost("manager/register")]
        public async Task<ResponseDto> RegisterManager([FromBody] UserRegistrationRequestDto request)
        {
            response.Result = await _userRegistrationService.RegisterManagerAsync(request);

            return response;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ResponseDto> Login([FromBody] UserLoginRequestDto request)
        {
            response.Result = await _authService.LoginAsync(request);

            return response;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = "https://localhost:7052/api/auth/signin-google";

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleCallback(string returnUrl = "", string remoteError = "")
        {
            var user = await _authService.GoogleCallBackAsync(returnUrl, remoteError);

            return Ok(user);
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ResponseDto> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            response.Result = await _authService.RefreshTokenAsync(request);

            return response;
        }

        [HttpPost("revoke-refresh-token")]
        [Authorize]
        public async Task<IActionResult> RevokeRefreshToken([FromBody] RevokeTokenRequestDto request)
        {
            var response = await _authService.RevokeRefreshTokenAsync(request);

            if (response != null && response.Message == "Refresh token revoked successfully")
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            await _userRegistrationService.ConfirmEmailAsync(token, email);

            return Redirect(_frontUrl);
        }

        [HttpPost("forgot-password")]
        [Authorize]
        public async Task<IActionResult> ForgotPassword()
        {
            await _authService.ForgotPasswordAsync();
            return Ok("Check your email.");
        }

        [HttpGet("reset-password")]
        public async Task<ResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            response.Result = await _authService.ResetPasswordAsync(resetPasswordDto);

            return response;
        }
    }
}