using Identity.Application.Configuration;
using Identity.Application.Dto;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Identity.Presentation.Controllers
{
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly IAuthService _authService;
        private readonly UrlSettings _urlSettings;
        private ResponseDto response;

        public AuthController(SignInManager<ApplicationUser> signInManager, IUserRegistrationService userRegistrationService, IAuthService authService, UserManager<ApplicationUser> userManager, IOptions<UrlSettings> urlSettings)
        {
            _signInManager = signInManager;
            _userRegistrationService = userRegistrationService;
            _authService = authService;
            _urlSettings = urlSettings.Value;
            _userManager = userManager;
            response = new ResponseDto();
        }
        [HttpGet("check-auth")]
        public async Task<IActionResult> CheckAuthentication()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                var identity = (ClaimsIdentity)User.Identity;
                var theme = User.FindFirst("theme")?.Value ??
                                   User.FindFirst("Theme")?.Value ?? "dark";
                return Ok(new
                {
                    isAuthenticated = true,
                    theme
                });
            }

            return Ok(new { isAuthenticated = false });
        }
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ResponseDto> Register([FromBody] UserRegistrationRequestDto request)
        {
            response.Result = await _userRegistrationService.RegisterAsync(request);

            return response;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ResponseDto> Login([FromBody] UserLoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new Exception("Invalid email or password.");

            await _signInManager.SignInAsync(user, isPersistent: false);
            response.Result = user;
            response.ReturnUrl = request.ReturnUrl;

            return response;
        }


        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(string token, string email)
        {
            await _userRegistrationService.ConfirmEmailAsync(token, email);

            return Redirect($"{_urlSettings.AuthFront}/theapp/#/theapp/login?message=Your email is verify. Welcom");
        }
        [HttpGet("redirect-to-login")]
        [AllowAnonymous]
        public IActionResult RedirectToFrontendLogin()
        {
            return Redirect("https://s-allina.github.io/theapp/#/theapp/login");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<bool> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            await _authService.ForgotPasswordAsync(forgotPasswordRequestDto.Email);
            return true;
        }


        [HttpPost("reset-password")]
        public async Task<ResponseDto> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            response.Result = await _authService.ResetPasswordAsync(resetPasswordDto);

            return response;
        }
    }
}