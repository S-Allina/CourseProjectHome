using Main.Domain.entities.item;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        private readonly new ApplicationDbContext _db;

        public ItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Item> UpdateItemAsync(Item item, CancellationToken cancellationToken = default)
        {
                var existingItem = await _db.Set<Item>()
                    .Include(i => i.FieldValues)
                    .FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken);

                if (existingItem == null)
                    throw new Exception("Item not found");

                _db.Entry(existingItem).CurrentValues.SetValues(item);
                existingItem.UpdatedAt = DateTime.UtcNow;

                await UpdateItemFieldValuesOptimizedAsync(existingItem, item.FieldValues, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
                return existingItem;
        }

        private async Task UpdateItemFieldValuesOptimizedAsync(Item existingItem, ICollection<ItemFieldValue> newFieldValues, CancellationToken cancellationToken)
        {
            var existingFieldValuesDict = existingItem.FieldValues.ToDictionary(f => f.InventoryFieldId);
            var newFieldValuesDict = newFieldValues.ToDictionary(f => f.InventoryFieldId);

            var fieldValuesToRemove = existingItem.FieldValues
                .Where(existingFieldValue => !newFieldValuesDict.ContainsKey(existingFieldValue.InventoryFieldId))
                .ToList();

            if (fieldValuesToRemove.Any())
            {
                _db.Set<ItemFieldValue>().RemoveRange(fieldValuesToRemove); 
                foreach (var fieldValueToRemove in fieldValuesToRemove)
                {
                    existingItem.FieldValues.Remove(fieldValueToRemove);
                }
            }

            foreach (var existingFieldValue in existingItem.FieldValues.Where(f => newFieldValuesDict.ContainsKey(f.InventoryFieldId)))
            {
                if (newFieldValuesDict.TryGetValue(existingFieldValue.InventoryFieldId, out var newFieldValue))
                {
                    _db.Entry(existingFieldValue).CurrentValues.SetValues(new
                    {
                        newFieldValue.TextValue,
                        newFieldValue.MultilineTextValue,
                        newFieldValue.NumberValue,
                        newFieldValue.FileUrl,
                        newFieldValue.BooleanValue,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            var fieldValuesToAdd = newFieldValues
                .Where(f => !existingFieldValuesDict.ContainsKey(f.InventoryFieldId))
                .ToList();

            if (fieldValuesToAdd.Any())
            {
                foreach (var fieldValueToAdd in fieldValuesToAdd)
                {
                    fieldValueToAdd.ItemId = existingItem.Id;
                    fieldValueToAdd.CreatedAt = DateTime.UtcNow;
                    fieldValueToAdd.UpdatedAt = DateTime.UtcNow;
                }
                await _db.Set<ItemFieldValue>().AddRangeAsync(fieldValuesToAdd, cancellationToken); 
                foreach (var fieldValueToAdd in fieldValuesToAdd)
                {
                    existingItem.FieldValues.Add(fieldValueToAdd);
                }
            }
        }

        public async Task<Item> UpdateItemWithVersionAsync(Item item, byte[] originalVersion, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingItem = await _db.Set<Item>()
                    .Include(i => i.FieldValues)
                    .FirstOrDefaultAsync(i => i.Id == item.Id && i.Version == originalVersion, cancellationToken);

                if (existingItem == null)
                    throw new Exception("Item not found or version mismatch");

                _db.Entry(existingItem).CurrentValues.SetValues(item);
                existingItem.UpdatedAt = DateTime.UtcNow;

                await UpdateItemFieldValuesOptimizedAsync(existingItem, item.FieldValues, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
                return existingItem;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("Item was modified by another user. Please refresh and try again.");
            }
        }

        public async Task<Item> GetByIdWithFieldValuesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Set<Item>()
                .Include(i => i.FieldValues)
                .Include(i => i.Inventory)
                    .ThenInclude(inv => inv.Fields)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<int> UpdateItemsAsync(IEnumerable<Item> items, CancellationToken cancellationToken = default)
        {
            var updatedCount = 0;

            foreach (var item in items)
            {
                await UpdateItemAsync(item, cancellationToken);
                updatedCount++;
            }

            return updatedCount;
        }
    }
}
