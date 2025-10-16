using Main.Domain.entities.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.Comments
{
    public class Like
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string UserId { get; set; }
        public DateTime LikedAt { get; set; } = DateTime.UtcNow;
    }
}
