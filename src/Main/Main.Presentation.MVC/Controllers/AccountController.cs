namespace Main.Presentation.MVC.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Mvc;

    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl = "/")
        {
            // Если пользователь не аутентифицирован, 
            // произойдет автоматический redirect на провайдера OpenID Connect
            if (!User.Identity.IsAuthenticated)
            {
                // Используем встроенный Challenge для перенаправления на провайдера OIDC
                return Challenge(
                    new AuthenticationProperties { RedirectUri = returnUrl },
                    OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<ActionResult> Logout()
        {
            // Выход из локальной аутентификации (куки) и OIDC схемы
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return Redirect("http://localhost:5173/theapp/#/theapp/logout?returnUrl=https://localhost:7004");
        }
    }
}
