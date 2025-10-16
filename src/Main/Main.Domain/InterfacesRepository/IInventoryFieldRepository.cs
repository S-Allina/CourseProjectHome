using Main.Domain.entities.inventory;
using Main.Domain.enums.inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Domain.InterfacesRepository
{
    public interface IInventoryFieldRepository : IBaseRepository<InventoryField>
    {
        //Task<IEnumerable<InventoryField>> GetByInventoryAsync(int inventoryId, CancellationToken cancellationToken = default);
        //Task ReorderFieldsAsync(int inventoryId, List<int> fieldIdsInOrder, CancellationToken cancellationToken = default);
        //Task<bool> CanAddFieldTypeAsync(int inventoryId, FieldType fieldType, CancellationToken cancellationToken = default);
    }
}
