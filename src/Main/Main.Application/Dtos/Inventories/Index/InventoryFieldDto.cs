using Main.Application.Helpers;
using Main.Domain.enums.inventory;

namespace Main.Application.Dtos.Inventories.Index
{
    public record InventoryFieldDto
    {
        public int Id { get; init; }
        public int InventoryId { get; set; }
        public required string Name { get; init; }
        public string Description { get; init; } = string.Empty;
        public string DescriptionHtml => MarkdownHelper.ConvertToHtml(Description);
        public FieldType FieldType { get; init; }
        public int OrderIndex { get; init; }
        public bool IsVisibleInTable { get; init; }
        public bool IsRequired { get; init; }
    }
}
