using Main.Domain.entities.common;
using Main.Domain.entities.inventory;

namespace Main.Domain.entities.Comments
{
    public class Comment : BaseEntity
    {
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public User Author { get; set; } = null!;
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
