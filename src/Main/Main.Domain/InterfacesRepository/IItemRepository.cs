using Main.Domain.entities.item;

namespace Main.Domain.InterfacesRepository
{

    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<Item> UpdateItemAsync(Item item, CancellationToken cancellationToken = default);
    }
}
