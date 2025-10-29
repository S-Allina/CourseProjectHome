using Main.Application.Dtos.Inventories.Index;
using Main.Application.Dtos.Items.Index;
using Main.Domain.enums.inventory;

namespace Main.Application.Dtos.Items.Create
{
    public record CreateItemDto
    {
        public int Id { get; set; }
        public string CreatedById { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public byte[] Version { get; set; }
        public int InventoryId { get; init; }
        public string CustomId { get; init; }
        public List<CreateItemFieldValueDto> FieldValues { get; init; } = new();
    }
    public class ItemFormViewModel
    {
        public bool IsEditMode { get; set; }
        public InventoryDto Inventory { get; set; }

        // Для создания
        public CreateItemDto CreateItem { get; set; }

        // Для редактирования  
        public ItemDto Item { get; set; }

        // Общие FieldValues для обеих форм
        public List<ItemFieldValueFormDto> FieldValues { get; set; } = new();
    }
    public class ItemFieldValueFormDto
    {
        public int InventoryFieldId { get; set; }
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
        public string CustomId { get; init; }
        public string CreatedById { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
        public byte[] Version { get; set; }

        // Значения полей
        public string TextValue { get; set; }
        public string MultilineTextValue { get; set; }
        public double? NumberValue { get; set; }
        public string FileUrl { get; set; }
        public bool? BooleanValue { get; set; }
    }
}
