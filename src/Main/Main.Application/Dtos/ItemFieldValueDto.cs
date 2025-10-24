using Main.Domain.enums.inventory;

namespace Main.Application.Dtos
{
    public record ItemFieldValueDto
    {
        public int InventoryFieldId { get; init; }
        public string FieldName { get; init; }
        public FieldType FieldType { get; init; }
        public string? TextValue { get; init; }
        public string? MultilineTextValue { get; init; }
        public decimal? NumberValue { get; init; }
        public string? FileUrl { get; init; }
        public bool? BooleanValue { get; init; }
    }
}
