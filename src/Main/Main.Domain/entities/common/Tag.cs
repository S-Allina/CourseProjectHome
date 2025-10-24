using Main.Domain.entities.inventory;

namespace Main.Domain.entities.common
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<InventoryTag> InventoryTags { get; set; }
    }
}
