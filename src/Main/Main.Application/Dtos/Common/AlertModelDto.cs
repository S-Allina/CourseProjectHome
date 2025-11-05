namespace Main.Application.Dtos.Common
{
    public record AlertModelDto
    {
        public string Type { get; init; } = "success";
        public string Title { get; init; } = "Success";
        public string Message { get; init; } = string.Empty;
    }
}
