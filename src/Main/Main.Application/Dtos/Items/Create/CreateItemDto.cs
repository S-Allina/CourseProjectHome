using Main.Application.Dtos.Inventories.Index;
using Main.Application.Dtos.Items.Index;
using Main.Domain.enums.inventory;

namespace Main.Application.Dtos.Items.Create
{
    public record CreateItemDto
    {
        public int Id { get; set; }
        public string CreatedById { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public byte[] Version { get; set; }
        public int InventoryId { get; init; }
        public string CustomId { get; init; }
        public List<CreateItemFieldValueDto> FieldValues { get; init; } = new();
    }
    public class ItemFormViewModel
    {
        public bool IsEditMode { get; set; }
        public InventoryTableDto Inventory { get; set; } = new();
        public CreateItemDto CreateItem { get; set; } = new();
        public ItemDto Item { get; set; } = new();
        public List<ItemFieldValueFormDto> FieldValues { get; set; } = new();
    }

    public class ItemFieldValueFormDto
    {
        public int InventoryFieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public FieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
        public string CustomId { get; init; } = string.Empty;
        public string CreatedById { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public byte[] Version { get; set; } 

        public string TextValue { get; set; } = string.Empty;
        public string MultilineTextValue { get; set; } = string.Empty;
        public double? NumberValue { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public bool? BooleanValue { get; set; }
    }
}
