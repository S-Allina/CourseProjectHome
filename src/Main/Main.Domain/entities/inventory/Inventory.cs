using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.item;
using System.ComponentModel.DataAnnotations;

namespace Main.Domain.entities.inventory
{
    public class Inventory : BaseEntity
    {
        public required string CustomIdFormat { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public required string OwnerId { get; set; }
        public User? Owner { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<InventoryField>? Fields { get; set; }
        public ICollection<InventoryAccess>? AccessList { get; set; }
        public ICollection<InventoryTag>? Tags { get; set; }
        public ICollection<Item>? Items { get; set; }
        public ICollection<ChatMessage>? ChatMessages { get; set; }
        public ICollection<Comment>? Comments { get; set; }

        public int CurrentSequence { get; set; } = 1;
        [Timestamp]
        public byte[] Version { get; set; }
    }
}
