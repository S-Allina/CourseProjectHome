using Main.Application.Dtos;
using Main.Domain.entities.inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Main.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto createDto, string ownerId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryDto>> GetAll(CancellationToken cancellationToken = default);
        Task<InventoryDto> GetById(int id, CancellationToken cancellationToken = default);
        Task<bool> DeleteInventoryAsync(int[] ids, CancellationToken cancellationToken = default);
        Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventoryDto, CancellationToken cancellationToken = default);
        Task<bool> HasWriteAccessAsync(int inventoryId, string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken = default);
        Task<List<InventorySearchResult>> GetInventoriesByTagAsync(string tagName);

    }
}
