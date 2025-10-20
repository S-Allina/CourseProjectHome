using Microsoft.OpenApi.Models;

namespace Main.Presentation.MVC.Constans
{
    public static class SwaggerConstants
    {
        public const string Title = "Product API";
        public const string Version = "v1";
        public const string Scheme = "Bearer";
        public const string Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"";
        public const string Name = "Authorization";
        public const ParameterLocation In = ParameterLocation.Header;
        public const SecuritySchemeType Type = SecuritySchemeType.Http;
        public const string BearerFormat = "JWT";
        public const ReferenceType TypeReference = ReferenceType.SecurityScheme;
    }
}
