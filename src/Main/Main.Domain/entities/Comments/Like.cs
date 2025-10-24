using Main.Domain.entities.common;
using Main.Domain.entities.item;

namespace Main.Domain.entities.Comments
{
    public class Like
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}
