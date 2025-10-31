using Main.Application.Dtos;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Domain.entities.inventory;
using Main.Domain.enums.Users;

namespace Main.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDetailsDto> CreateInventoryAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryTableDto>> GetAll(CancellationToken cancellationToken = default);
        Task<InventoryDetailsDto> GetById(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteInventoryAsync(int[] ids, CancellationToken cancellationToken = default);
        Task<InventoryDetailsDto> UpdateInventoryAsync(InventoryDetailsDto inventoryDto, CancellationToken cancellationToken = default);
        Task<bool> HasWriteAccessAsync(int inventoryId, AccessLevel accessLevel, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken = default);
        Task<List<InventorySearchResult>> GetInventoriesByTagAsync(string tagName);
        Task<IEnumerable<InventoryTableDto>> GetUserInventoriesAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryTableDto>> GetSharedInventoriesAsync(CancellationToken cancellationToken = default);
        
            Task<IEnumerable<InventoryTableDto>> GetRecentInventoriesAsync(int count, CancellationToken cancellationToken);
            Task<IEnumerable<InventoryTableDto>> GetPopularInventoriesAsync(int count, CancellationToken cancellationToken);

    }
}
