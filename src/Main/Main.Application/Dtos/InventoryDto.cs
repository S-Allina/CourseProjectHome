using Main.Application.Helpers;

namespace Main.Application.Dtos
{
    public record InventoryDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string DescriptionHtml => MarkdownHelper.ConvertToHtml(Description);
        public string DescriptionPreview => MarkdownHelper.TruncateWithMarkdown(Description);
        public int? CategoryId { get; init; }
        public string OwnerId { get; init; }
        public string ImageUrl { get; init; }
        public bool IsPublic { get; init; }
        public string CustomIdFormat { get; init; }
        public byte[] Version { get; set; }
        public DateTime CreatedAt { get; init; }
        public List<string> Tags { get; init; } = new();
        public List<InventoryFieldDto> Fields { get; init; } = new();
        public List<InventoryAccessDto> AccessList { get; set; } = new();

    }
}
