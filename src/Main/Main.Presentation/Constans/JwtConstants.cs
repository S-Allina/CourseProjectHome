namespace Product.Presentation.Constans
{
    public static class JwtConstants
    {
        public const bool ValidateIssuer = true;
        public const bool ValidateAudience = false;
        public const bool ValidateLifetime = true;
        public const bool ValidateIssuerSigningKey = true;
        public const string IssuerPath = "JwtSettings:validIssuer";
        public const string AudiencePath = "JwtSettings:validAudience";
        public const string KeyPath = "JwtSettings:key";
        public const char AudienceSeparator = ';';
        public static readonly TimeSpan ClockSkew = TimeSpan.Zero;
    }
}
