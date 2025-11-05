namespace Main.Application.Dtos.Items.Index
{
    public record ItemFieldValueFormDto(bool IsRequired, string CreatedById, DateTime CreatedAt, DateTime? UpdatedAt, byte[]? Version) : ItemFieldValueDto
    {
        public string CustomId { get; set; } = string.Empty;
    }
}
