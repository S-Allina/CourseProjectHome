using Main.Domain.entities.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.inventory
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
    }
}
