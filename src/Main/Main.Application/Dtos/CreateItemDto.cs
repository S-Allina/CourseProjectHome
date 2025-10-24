using Main.Domain.enums.inventory;

namespace Main.Application.Dtos
{
    public record CreateItemDto
    {
        public int InventoryId { get; init; }
        public string CustomId { get; init; }
        public List<CreateItemFieldValueDto> FieldValues { get; init; } = new();
    }

    public record CreateItemFieldValueDto
    {
        public int InventoryFieldId { get; init; }
        public string FieldName { get; init; }
        public FieldType FieldType { get; init; }
        public bool IsRequired { get; init; }

        // Значения полей
        public string TextValue { get; init; }
        public string MultilineTextValue { get; init; }
        public decimal? NumberValue { get; init; }
        public string FileValue { get; init; }
        public bool? BooleanValue { get; init; }
    }
}
