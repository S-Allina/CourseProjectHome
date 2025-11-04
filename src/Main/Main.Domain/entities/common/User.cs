using Main.Domain.entities.Comments;
using Main.Domain.entities.inventory;
using Main.Domain.entities.item;

namespace Main.Domain.entities.common
{
    public class User:BaseEntity
    {
        public new required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public ICollection<InventoryAccess> InventoryAccesses { get; set; } = new List<InventoryAccess>();
        public ICollection<Item> CreatedItems { get; set; } = new List<Item>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
