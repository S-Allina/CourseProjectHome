// Controllers/AccountController.cs в Auth проекте
using Identity.Domain.Entity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Web;

namespace Identity.Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            // Перенаправляем на React фронтенд для логина
            var decodedReturnUrl = HttpUtility.UrlDecode(returnUrl);
            var reactLoginUrl = $"http://localhost:5173/theapp/#/theapp/login?returnUrl={Uri.EscapeDataString(decodedReturnUrl)}";
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

            // Если логин не удался, возвращаем на React с ошибкой
            var reactErrorUrl = $"http://localhost:5173/login?error=invalid_credentials&returnUrl={Uri.EscapeDataString(model.ReturnUrl)}";
            return Redirect(reactErrorUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();
            return Redirect("http://localhost:5173");
        }
    }

    public class LoginInputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}