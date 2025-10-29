using Main.Application.Dtos;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Domain.entities.inventory;
using Main.Domain.enums.Users;

namespace Main.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryDto>> GetAll(CancellationToken cancellationToken = default);
        Task<InventoryDto> GetById(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteInventoryAsync(int[] ids, CancellationToken cancellationToken = default);
        Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventoryDto, CancellationToken cancellationToken = default);
        Task<bool> HasWriteAccessAsync(int inventoryId, AccessLevel accessLevel, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken = default);
        Task<List<InventorySearchResult>> GetInventoriesByTagAsync(string tagName);
        Task<IEnumerable<InventoryDto>> GetUserInventoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryDto>> GetSharedInventoriesAsync(CancellationToken cancellationToken = default);
        
            Task<IEnumerable<InventoryDto>> GetRecentInventoriesAsync(int count, CancellationToken cancellationToken);
            Task<IEnumerable<InventoryDto>> GetPopularInventoriesAsync(int count, CancellationToken cancellationToken);

    }
}
