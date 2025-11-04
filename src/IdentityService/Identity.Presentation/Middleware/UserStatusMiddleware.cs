using Identity.Application.Interfaces;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Presentation.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public UserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        private static readonly string[] _allowedPaths = ["/login", "/register", "/reset-password", "/logout", "/check-auth"];

        public async Task InvokeAsync(HttpContext context, IUserService userService, ICurrentUserService currentUserService)
        {
            if (_allowedPaths.Any(path => context.Request.Path.Value.Contains(path)))
            {
                await _next(context);
                return;
            }
            if (!context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var userId = currentUserService.GetUserId();
            var user = await currentUserService.GetCurrentUserAsync();
            var userResponse = string.IsNullOrEmpty(userId) ? null : await userService.GetByIdAsync(userId, new CancellationToken());

            if (userResponse == null || userResponse.Status == Statuses.Blocked)
            {
                var statusMessage = userResponse == null ? "deleted" : userResponse?.Status.ToString().ToLower();
                await context.SignOutAsync();
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync($"User account has been {statusMessage}");
                return;
            }

            await _next(context);
        }
    }
}
