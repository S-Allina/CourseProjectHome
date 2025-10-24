namespace Identity.Application.Dto
{
    public record UserRegistrationResponseDto
    {
        public string Id { get; init; }
        public string Email { get; init; }
        public string Message { get; init; }
    }
}
