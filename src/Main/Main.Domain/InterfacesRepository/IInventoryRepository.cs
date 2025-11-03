using Main.Domain.entities.inventory;
using Main.Domain.enums.Users;

namespace Main.Domain.InterfacesRepository
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default);
        Task<bool> HasUserAccessAsync(int inventoryId, string userId, AccessLevel accessLevel, CancellationToken cancellationToken = default);
    }
}
