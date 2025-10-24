namespace Identity.Application.Dto
{
    public record ResetPasswordDto
    {
        public string Token { get; init; }
        public string Email { get; init; }
        public string NewPassword { get; init; }
        public object ConfirmPassword { get; init; }
    }
}
