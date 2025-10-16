using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.InterfacesRepository
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
    }
}
