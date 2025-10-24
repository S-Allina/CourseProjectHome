using Main.Domain.entities.inventory;

namespace Main.Domain.InterfacesRepository
{
    public interface IInventoryAccessRepository
    {
        Task GrantAccessAsync(int inventoryId, string userId, string grantedById, int accessLevel = 2, CancellationToken cancellationToken = default);
        Task RevokeAccessAsync(int inventoryId, string userId, CancellationToken cancellationToken = default);
        Task<bool> HasAccessAsync(int inventoryId, string userId, int minAccessLevel = 1, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryAccess>> GetAccessListAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Inventory>> GetInventoriesWithAccessAsync(string userId, CancellationToken cancellationToken = default);
    }

}
