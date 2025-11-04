namespace Identity.Application.Dto
{
    public record UserRegistrationResponseDto
    {
        public required string Id { get; init; }
        public required string Email { get; init; }
        public string? Message { get; init; }
    }
}
