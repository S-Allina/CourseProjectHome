using Identity.Application.Interfaces;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Authentication;

namespace Identity.Presentation.Middleware
{
    public class UserStatusMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly string[] _allowedPaths = ["/login", "/register", "/reset-password", "/logout", "/check-auth"];

        public UserStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService, ICurrentUserService currentUserService)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.Request.Path.Value != null &&
                _allowedPaths.Any(path => context.Request.Path.Value.Contains(path, StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            if (context.User?.Identity?.IsAuthenticated != true)
            {
                await _next(context);
                return;
            }

            var userId = currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                await _next(context);
                return;
            }

            var userResponse = await userService.GetByIdAsync(userId, CancellationToken.None);

            if (userResponse == null || userResponse.Status == Statuses.Blocked)
            {
                var statusMessage = userResponse == null ? "deleted" : userResponse.Status.ToString().ToLowerInvariant();

                await context.SignOutAsync();

                context.Response.StatusCode = 403;
                await context.Response.WriteAsync($"User account has been {statusMessage}");
                return;
            }

            await _next(context);
        }
    }
}
