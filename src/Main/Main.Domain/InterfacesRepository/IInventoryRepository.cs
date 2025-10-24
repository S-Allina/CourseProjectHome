using Main.Domain.entities.inventory;

namespace Main.Domain.InterfacesRepository
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
    }
}
