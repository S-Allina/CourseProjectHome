using Main.Domain.entities.item;

namespace Main.Domain.InterfacesRepository
{

    public interface IItemRepository : IBaseRepository<Item>
    {
        //Task<InventoryStats> GetInventoryStatsAsync(int inventoryId, CancellationToken cancellationToken = default);
        Task<Item> UpdateItemAsync(Item item, CancellationToken cancellationToken = default);
    }
}
