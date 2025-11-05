using Main.Application.Dtos.Items.Create;
using Main.Application.Dtos.Items.Index;

namespace Main.Application.Interfaces
{
    public interface IItemService
    {
        Task<ItemDto> CreateAsync(CreateItemDto createDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<ItemDto>> GetByInventoryAsync(int id, CancellationToken cancellationToken = default);
        Task<int> DeleteItemAsync(int[] ids, CancellationToken cancellationToken = default);
        Task<ItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ItemDto> UpdateItemAsync(ItemDto itemDto, CancellationToken cancellationToken = default);
    }
}
