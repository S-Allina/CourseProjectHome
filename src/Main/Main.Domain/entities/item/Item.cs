using Main.Domain.entities.Comments;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.item
{
    public class Item : BaseEntity  
    {
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public string CustomId { get; set; }
        public string CreatedById { get; set; }
        [Timestamp]
        public byte[] Version { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ItemFieldValue> FieldValues { get; set; } = new List<ItemFieldValue>();
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
