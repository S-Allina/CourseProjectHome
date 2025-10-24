using Main.Domain.entities.common;
using Main.Domain.enums.Users;

namespace Main.Domain.entities.inventory
{
    public class InventoryAccess : BaseEntity
    {
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public string GrantedById { get; set; } = string.Empty;
        public User GrantedBy { get; set; } = null!;

        public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
        public AccessLevel AccessLevel { get; set; } = AccessLevel.ReadOnly;
    }
}
