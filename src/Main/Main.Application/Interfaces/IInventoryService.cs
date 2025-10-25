using Main.Application.Dtos;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Domain.entities.inventory;

namespace Main.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryDto>> GetAll(CancellationToken cancellationToken = default);
        Task<InventoryDto> GetById(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteInventoryAsync(int[] ids, CancellationToken cancellationToken = default);
        Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventoryDto, CancellationToken cancellationToken = default);
        Task<bool> HasWriteAccessAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken = default);
        Task<List<InventorySearchResult>> GetInventoriesByTagAsync(string tagName);
        Task<IEnumerable<InventoryDto>> GetUserInventoriesAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryDto>> GetSharedInventoriesAsync(string userId, CancellationToken cancellationToken = default);

    }
}
