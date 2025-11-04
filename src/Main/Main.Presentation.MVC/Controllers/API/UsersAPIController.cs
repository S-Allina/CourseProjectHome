using Main.Application.Dtos.Common;
using Main.Application.Interfaces;
using Main.Domain.entities.common;
using Main.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Main.Presentation.MVC.Controllers.API
{
    [ApiController]
    [Route("api/users")]
    public class UsersAPIController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersAPIController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
                var message = await _usersService.CreateUser(request);
                return Ok(new { message });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] string[] ids)
        {
            var message = await _usersService.DeleteUsersAsync(ids);
            return Ok(new { message });
        }

        [HttpPost("blocked-users")]
        public IActionResult NotifyBlockedUsers([FromBody] string[] blockedUserIds)
        {
            var isBlock = _usersService.CheckBlock(blockedUserIds);
            if (!isBlock) return Ok();

            return RedirectToAction("Logout", "Account");
        }

        [HttpPost("UpdateTheme")]
        public async Task<IActionResult> UpdateTheme([FromBody] string theme)
        {
            if (User?.Identity?.IsAuthenticated==true)
            {
                Response.Cookies.Append("user_theme", theme, new CookieOptions
                {
                    Expires = DateTimeOffset.Now.AddYears(1),
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                });

                var identity = (ClaimsIdentity)User.Identity;
                var themeClaim = identity.FindFirst("theme");

                if (themeClaim != null)
                    identity.RemoveClaim(themeClaim);

                identity.AddClaim(new Claim("theme", theme));
                await HttpContext.SignInAsync(User);

                return Ok(new { success = true, theme });
            }
            return BadRequest();
        }
    }
}
