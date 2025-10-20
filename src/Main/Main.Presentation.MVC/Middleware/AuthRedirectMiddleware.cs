namespace Main.Presentation.MVC.Middleware
{
    public class AuthRedirectMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthRedirectMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/auth") &&
                !context.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect("/auth/login");
                return;
            }

            await _next(context);
        }
    }
}
