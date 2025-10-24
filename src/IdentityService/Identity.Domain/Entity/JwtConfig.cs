namespace Identity.Domain.Entity
{
    public record JwtSettings
    {
        public string? Key { get; init; }
        public string ValidIssuer { get; init; }
        public string ValidAudience { get; init; }
        public double Expires { get; init; }
    }
}
