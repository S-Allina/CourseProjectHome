using Main.Domain.entities.item;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext db) : base(db) { }

        public async Task<Item> UpdateItemAsync(Item item, CancellationToken cancellationToken = default)
        {
            try
            {
                // Сначала загружаем существующую сущность
                var existingItem = await _db.Set<Item>()
                    .FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken);

                if (existingItem == null)
                    throw new Exception("Item not found");

                // Копируем значения из входной сущности в загруженную
                _db.Entry(existingItem).CurrentValues.SetValues(item);
                // Сохраняем изменения
                await _db.SaveChangesAsync(cancellationToken);
                return existingItem;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
