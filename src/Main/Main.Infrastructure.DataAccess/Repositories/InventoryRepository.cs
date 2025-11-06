using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private new readonly ApplicationDbContext _db;
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
                return await GetAllAsync(null, cancellationToken, "Fields", "Tags.Tag", "Category");

            return await dbSet
                .Include(i => i.Fields!)
                .Include(i => i.Tags!)
                    .ThenInclude(t => t.Tag!)
                .Include(i => i.Category!)
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm) ||
                           EF.Functions.FreeText(i.Description, searchTerm))
                .ToListAsync(cancellationToken);
        }

        public async Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            var existingInventory = await _db.Set<Inventory>()
                .Include(i => i.Fields)
                .FirstOrDefaultAsync(i => i.Id == inventory.Id, cancellationToken);

            if (existingInventory == null)
                throw new Exception("Inventory not found");
            
            if (Convert.ToBase64String(existingInventory.Version) != Convert.ToBase64String(inventory.Version))
                throw new DbUpdateConcurrencyException("Инвентарь изменён другим пользователем обновите страницу.");

            _db.Entry(existingInventory).CurrentValues.SetValues(inventory);
            existingInventory.UpdatedAt = DateTime.UtcNow;

            if (inventory.Fields != null)
            {
                await UpdateInventoryFieldsOptimizedAsync(existingInventory, inventory.Fields, cancellationToken);
            }

            await _db.SaveChangesAsync(cancellationToken);
            return existingInventory;
        }

        private async Task UpdateInventoryFieldsOptimizedAsync(Inventory existingInventory, ICollection<InventoryField> newFields, CancellationToken cancellationToken)
        {
            if (existingInventory == null)
                throw new ArgumentNullException(nameof(existingInventory));

            if (newFields == null)
                throw new ArgumentNullException(nameof(newFields));

            existingInventory.Fields ??= new List<InventoryField>();

            var existingFieldsDict = existingInventory.Fields.ToDictionary(f => f.Id);
            var newFieldsDict = newFields.Where(f => f.Id != 0).ToDictionary(f => f.Id);

            var fieldsToRemove = existingInventory.Fields
                .Where(existingField => !newFieldsDict.ContainsKey(existingField.Id))
                .ToList();

            if (fieldsToRemove.Count > 0)
            {
                _db.Set<InventoryField>().RemoveRange(fieldsToRemove);
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    existingInventory.Fields.Remove(fieldToRemove);
                }
            }

            var existingFieldsToUpdate = existingInventory.Fields
                .Where(f => newFieldsDict.ContainsKey(f.Id))
                .ToList();

            foreach (var existingField in existingFieldsToUpdate)
            {
                if (newFieldsDict.TryGetValue(existingField.Id, out var newField))
                {
                    _db.Entry(existingField).CurrentValues.SetValues(newField);
                }
            }

            var fieldsToAdd = newFields.Where(f => f.Id == 0).ToList();
            if (fieldsToAdd.Count > 0)
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
