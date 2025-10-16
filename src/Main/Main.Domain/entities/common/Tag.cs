using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.common
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<InventoryTag> InventoryTags { get; set; }
    }
}
