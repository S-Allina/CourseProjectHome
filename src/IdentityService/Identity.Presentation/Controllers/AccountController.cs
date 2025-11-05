using Identity.Application.Configuration;
using Identity.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Web;

namespace Identity.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UrlSettings _urlSettings;

        public AccountController(SignInManager<ApplicationUser> signInManager, IOptions<UrlSettings> urlSettings)
        {
            _signInManager = signInManager;
            _urlSettings = urlSettings.Value;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var decodedReturnUrl = HttpUtility.UrlDecode(returnUrl);
            var reactLoginUrl = $"{_urlSettings.AuthFront}/theapp/#/theapp/login?returnUrl={Uri.EscapeDataString(decodedReturnUrl)}";
            return Redirect(reactLoginUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username, model.Password, model.RememberLogin, false);

                if (result.Succeeded)
                {
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return Redirect("~/");
                }
            }

            var reactErrorUrl = $"{_urlSettings.AuthFront}/login?error=invalid_credentials&returnUrl={Uri.EscapeDataString(model.ReturnUrl)}";
            return Redirect(reactErrorUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            return Redirect(_urlSettings.Main);
        }

    }

    public class LoginInputModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool RememberLogin { get; set; }
        public required string ReturnUrl { get; set; }
    }
}