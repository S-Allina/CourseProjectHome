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
        public Inventory Inventory { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
   
        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }

        public int? ParentMessageId { get; set; }
        public ChatMessage? ParentMessage { get; set; }
        public ICollection<ChatMessage> Replies { get; set; } = new List<ChatMessage>();
    }
}
