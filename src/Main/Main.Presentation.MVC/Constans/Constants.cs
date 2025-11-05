using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Main.Presentation.MVC.Constants
{
    public static class AuthConstants
    {
        public const string DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        public const string ChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        public const string ClientId = "MainMVCApp";
        public const string ResponseType = "code";
        public const string SignedOutCallbackPath = "/signout-callback-oidc";
        public const string RemoteSignOutPath = "/signout-oidc";

        public static readonly string[] Scopes =
        {
            "openid", "profile", "email", "api1", "roles", "theme", "language"
        };
    }

    public static class CorsConstants
    {
        public const string PolicyName = "CorsPolicy";
    }

    public static class SwaggerConstants
    {
        public const string Version = "v1";
        public const string Title = "Main API";
        public const string SecurityScheme = "oauth2";

        public static readonly string[] SecurityScopes = { "openid", "profile", "email" };
    }

    public static class CultureConstants
    {
        public static readonly string[] SupportedCultures = { "en", "ru" };
        public const string DefaultCulture = "en";
    }

    public static class HttpClientConstants
    {
        public const string AuthService = "AuthService";
    }
}