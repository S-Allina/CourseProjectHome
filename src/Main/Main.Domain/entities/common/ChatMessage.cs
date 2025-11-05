using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.common
{
    public class ChatMessage : BaseEntity
    {
        public int InventoryId { get; set; }
        public Inventory? Inventory { get; set; }

        public required string UserId { get; set; }
        public string? UserName { get; set; }
   
        public required string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
