using Main.Domain.enums.inventory;

namespace Main.Application.Dtos.Items.Create
{
    public record CreateItemDto
    {
        public int InventoryId { get; init; }
        public string CustomId { get; init; }
        public List<CreateItemFieldValueDto> FieldValues { get; init; } = new();
    }
}
