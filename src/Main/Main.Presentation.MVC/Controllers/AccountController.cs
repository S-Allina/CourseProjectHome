using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
namespace Main.Presentation.MVC.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string returnUrl = "/")
        {
            if (User?.Identity?.IsAuthenticated == false)
            {
                return Challenge(
                    new AuthenticationProperties { RedirectUri = returnUrl },
                    OpenIdConnectDefaults.AuthenticationScheme);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Index", "Home")
                },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
