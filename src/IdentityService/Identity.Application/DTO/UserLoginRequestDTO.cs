namespace Identity.Application.Dto
{
    public record UserLoginRequestDto
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string ReturnUrl { get; init; }
    }
}
