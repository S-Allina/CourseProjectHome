using Main.Domain.enums.inventory;

namespace Main.Application.Dtos.Items.Index
{
    public record ItemFieldValueDto
    {
        public int InventoryFieldId { get; init; }
        public required string FieldName { get; init; }
        public FieldType FieldType { get; init; }

        public string? TextValue { get; init; }
        public string? MultilineTextValue { get; init; }
        public double? NumberValue { get; init; }
        public string? FileUrl { get; init; }
        public bool? BooleanValue { get; init; }
    }
}
