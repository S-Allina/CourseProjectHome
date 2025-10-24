namespace Identity.Application.Dto
{
    public record UserLoginRequestDto
    {
        public string Email { get; init; }
        public string Password { get; init; }
        public string ReturnUrl { get; set; }
    }
}
