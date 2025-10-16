using Main.Domain.entities.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.inventory
{
    public class InventoryTag
    {
        public int InventoryId { get; set; }
        public Inventory Inventory { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
