using Main.Domain.enums.inventory;

namespace Main.Application.Dtos.Inventories.Create
{
    public class CreateInventoryFieldDto
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public FieldType FieldType { get; init; }
        public int OrderIndex { get; set; }
        public bool IsVisibleInTable { get; init; }
        public bool IsRequired { get; init; }
    }
}
