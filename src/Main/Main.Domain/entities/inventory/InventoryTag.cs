using Main.Domain.entities.common;

namespace Main.Domain.entities.inventory
{
    public class InventoryTag
    {
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
