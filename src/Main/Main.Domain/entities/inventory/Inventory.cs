using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.item;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.inventory
{
    public class Inventory : BaseEntity
    {
        public string CustomIdFormat { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string OwnerId { get; set; } 
        public string ImageUrl { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<InventoryField> Fields { get; set; } = new List<InventoryField>();
        public ICollection<InventoryAccess> AccessList { get; set; } = new List<InventoryAccess>();
        public ICollection<InventoryTag> Tags { get; set; } = new List<InventoryTag>();
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public int CurrentSequence { get; set; } = 1;
        [Timestamp]
        public byte[] Version { get; set; }
    }
}
