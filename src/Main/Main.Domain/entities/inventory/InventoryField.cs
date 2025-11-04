using Main.Domain.entities.common;
using Main.Domain.enums.inventory;

namespace Main.Domain.entities.inventory
{
    public class InventoryField : BaseEntity
    {
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; }

        public required string Name { get; set; }
        public string? Description { get; set; }
        public FieldType FieldType { get; set; }
        public int OrderIndex { get; set; }
        public bool IsVisibleInTable { get; set; }
        public bool IsRequired { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
