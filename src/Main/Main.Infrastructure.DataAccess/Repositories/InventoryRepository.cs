using Main.Domain.entities.inventory;
using Main.Domain.enums.Users;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private readonly new ApplicationDbContext _db;
        public InventoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Inventory>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync(null, cancellationToken);

            return await dbSet
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm) ||
                           EF.Functions.FreeText(i.Description, searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Inventory>> SearchWithDetailsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync(null, cancellationToken, "Fields","Tags.Tag","Category");

            return await dbSet
                .Include(i => i.Fields)
                .Include(i => i.Tags)
                    .ThenInclude(t => t.Tag)
                .Include(i => i.Category)
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm) ||
                           EF.Functions.FreeText(i.Description, searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasUserAccessAsync(int inventoryId, string userId, AccessLevel accessLevel, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .AnyAsync(i => i.Id == inventoryId &&
                              (i.OwnerId == userId ||
                               i.IsPublic ||
                               i.AccessList.Any(a => a.UserId == userId && (int)a.AccessLevel >= (int)accessLevel)),
                          cancellationToken);
        }

        public async Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
                var existingInventory = await _db.Set<Inventory>()
                    .Include(i => i.Fields)
                    .FirstOrDefaultAsync(i => i.Id == inventory.Id, cancellationToken);

                if (existingInventory == null)
                    throw new Exception("Inventory not found");

            if (existingInventory.Version != inventory.Version)
                throw new DbUpdateConcurrencyException("Инвентарь изменён другим пользователем обновите страницу.");

                _db.Entry(existingInventory).CurrentValues.SetValues(inventory);
                existingInventory.UpdatedAt = DateTime.UtcNow;

                await UpdateInventoryFieldsOptimizedAsync(existingInventory, inventory.Fields, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
                return existingInventory;
        }

        private async Task UpdateInventoryFieldsOptimizedAsync(Inventory existingInventory, ICollection<InventoryField> newFields, CancellationToken cancellationToken)
        {
            var existingFieldsDict = existingInventory.Fields.ToDictionary(f => f.Id);
            var newFieldsDict = newFields.Where(f => f.Id != 0).ToDictionary(f => f.Id);

            var fieldsToRemove = existingInventory.Fields
                .Where(existingField => !newFieldsDict.ContainsKey(existingField.Id))
                .ToList();

            if (fieldsToRemove.Any())
            {
                _db.Set<InventoryField>().RemoveRange(fieldsToRemove);
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    existingInventory.Fields.Remove(fieldToRemove);
                }
            }

            foreach (var existingField in existingInventory.Fields.Where(f => newFieldsDict.ContainsKey(f.Id)))
            {
                if (newFieldsDict.TryGetValue(existingField.Id, out var newField))
                {
                    _db.Entry(existingField).CurrentValues.SetValues(newField);
                }
            }

            var fieldsToAdd = newFields.Where(f => f.Id == 0).ToList();
            if (fieldsToAdd.Any())
            {
                foreach (var fieldToAdd in fieldsToAdd)
                {
                    fieldToAdd.InventoryId = existingInventory.Id;
                    fieldToAdd.CreatedAt = DateTime.UtcNow;
                }
                await _db.Set<InventoryField>().AddRangeAsync(fieldsToAdd, cancellationToken);
                foreach (var fieldToAdd in fieldsToAdd)
                {
                    existingInventory.Fields.Add(fieldToAdd);
                }
            }
        }
    }
}
