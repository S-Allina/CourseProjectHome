using Main.Domain.entities.inventory;

namespace Main.Domain.InterfacesRepository
{
    public interface IInventoryFieldRepository : IBaseRepository<InventoryField>
    {
        //Task<IEnumerable<InventoryField>> GetByInventoryAsync(int inventoryId, CancellationToken cancellationToken = default);
        //Task ReorderFieldsAsync(int inventoryId, List<int> fieldIdsInOrder, CancellationToken cancellationToken = default);
        //Task<bool> CanAddFieldTypeAsync(int inventoryId, FieldType fieldType, CancellationToken cancellationToken = default);
    }
}
