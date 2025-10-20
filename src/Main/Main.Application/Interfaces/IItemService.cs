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
    public interface IItemService
    {
        Task<ItemDto> CreateAsync(CreateItemDto createDto, string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ItemDto>> GetByInventoryAsync(int id, CancellationToken cancellationToken = default);
        Task<int> DeleteItemAsync(int[] ids, CancellationToken cancellationToken = default);
        Task<ItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ItemDto> UpdateItemAsync(ItemDto itemDto, CancellationToken cancellationToken = default);
        //Task<IEnumerable<InventoryDto>> GetAll(CancellationToken cancellationToken = default);
        //Task<InventoryDto> GetById(int id, CancellationToken cancellationToken = default);
        //Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        //Task<InventoryDto> UpdateAsync(InventoryDto inventoryDto, CancellationToken cancellationToken = default);
    }
}
