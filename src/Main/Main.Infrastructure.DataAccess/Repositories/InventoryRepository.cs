using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _db;
        public InventoryRepository(ApplicationDbContext db) : base(db) {
            _db = db;

        }

        public async Task<IEnumerable<Inventory>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync(null, null, cancellationToken);

            return await dbSet
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm) ||
                           EF.Functions.FreeText(i.Description, searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Inventory>> SearchWithDetailsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync(null, "Fields,Tags.Tag,Category", cancellationToken);

            return await dbSet
                .Include(i => i.Fields)
                .Include(i => i.Tags)
                    .ThenInclude(t => t.Tag)
                .Include(i => i.Category)
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm) ||
                           EF.Functions.FreeText(i.Description, searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            try
            {
                // Сначала загружаем существующую сущность
                var existingInventory = await _db.Set<Inventory>()
                    .FirstOrDefaultAsync(i => i.Id == inventory.Id, cancellationToken);

                if (existingInventory == null)
                    throw new Exception("Inventory not found");

                // Копируем значения из входной сущности в загруженную
                _db.Entry(existingInventory).CurrentValues.SetValues(inventory);
                existingInventory.OwnerId = "fgvsfgv";
                // Сохраняем изменения
                await _db.SaveChangesAsync(cancellationToken);
                return existingInventory;
            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}
