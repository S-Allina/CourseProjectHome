using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.entities.inventory
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Inventory> Inventories { get; set; }
    }
}
