using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.item
{
    public class ItemFieldValue : BaseEntity
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public int InventoryFieldId { get; set; }
        public InventoryField InventoryField { get; set; }

        public string? TextValue { get; set; }
        public string? MultilineTextValue { get; set; }
        public decimal? NumberValue { get; set; }
        public string? FileUrl { get; set; }
        public bool? BooleanValue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
