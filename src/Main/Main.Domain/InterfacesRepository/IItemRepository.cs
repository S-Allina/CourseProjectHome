using Main.Domain.entities.item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.InterfacesRepository
{
    
    public interface IItemRepository : IBaseRepository<Item>
    {
        //Task<InventoryStats> GetInventoryStatsAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<Item> UpdateItemAsync(Item item, CancellationToken cancellationToken = default);
    }
}
