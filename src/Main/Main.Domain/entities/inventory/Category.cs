using Main.Domain.entities.common;

namespace Main.Domain.entities.inventory
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
    }
}
