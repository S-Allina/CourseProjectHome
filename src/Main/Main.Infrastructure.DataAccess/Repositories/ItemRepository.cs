using Main.Domain.entities.item;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        private readonly ApplicationDbContext _db;

        public ItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Item> UpdateItemAsync(Item item, CancellationToken cancellationToken = default)
        {
            try
            {
                // Загружаем существующую сущность с включением FieldValues
                var existingItem = await _db.Set<Item>()
                    .Include(i => i.FieldValues)
                    .FirstOrDefaultAsync(i => i.Id == item.Id, cancellationToken);

                if (existingItem == null)
                    throw new Exception("Item not found");

                // Обновляем скалярные свойства
                _db.Entry(existingItem).CurrentValues.SetValues(item);
                existingItem.UpdatedAt = DateTime.UtcNow;

                // Обрабатываем изменения в коллекции FieldValues
                await UpdateItemFieldValuesOptimizedAsync(existingItem, item.FieldValues, cancellationToken);

                // ✅ ВСЕ изменения сохраняются ОДНИМ запросом
                await _db.SaveChangesAsync(cancellationToken);
                return existingItem;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateItemFieldValuesOptimizedAsync(Item existingItem, ICollection<ItemFieldValue> newFieldValues, CancellationToken cancellationToken)
        {
            var existingFieldValuesDict = existingItem.FieldValues.ToDictionary(f => f.InventoryFieldId);
            var newFieldValuesDict = newFieldValues.ToDictionary(f => f.InventoryFieldId);

            // 1. Удаляем значения полей, которых больше нет - ОДНА операция для всех
            var fieldValuesToRemove = existingItem.FieldValues
                .Where(existingFieldValue => !newFieldValuesDict.ContainsKey(existingFieldValue.InventoryFieldId))
                .ToList();

            if (fieldValuesToRemove.Any())
            {
                _db.Set<ItemFieldValue>().RemoveRange(fieldValuesToRemove); // ✅ Bulk remove
                foreach (var fieldValueToRemove in fieldValuesToRemove)
                {
                    existingItem.FieldValues.Remove(fieldValueToRemove);
                }
            }

            // 2. Обновляем существующие значения полей
            foreach (var existingFieldValue in existingItem.FieldValues.Where(f => newFieldValuesDict.ContainsKey(f.InventoryFieldId)))
            {
                if (newFieldValuesDict.TryGetValue(existingFieldValue.InventoryFieldId, out var newFieldValue))
                {
                    // Копируем только значения, не затрагивая ключи и метаданные
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

            // 3. Добавляем новые значения полей - ОДНА операция для всех
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
                await _db.Set<ItemFieldValue>().AddRangeAsync(fieldValuesToAdd, cancellationToken); // ✅ Bulk add
                foreach (var fieldValueToAdd in fieldValuesToAdd)
                {
                    existingItem.FieldValues.Add(fieldValueToAdd);
                }
            }
        }

        // Дополнительный метод для обновления с проверкой версии (optimistic concurrency)
        public async Task<Item> UpdateItemWithVersionAsync(Item item, byte[] originalVersion, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingItem = await _db.Set<Item>()
                    .Include(i => i.FieldValues)
                    .FirstOrDefaultAsync(i => i.Id == item.Id && i.Version == originalVersion, cancellationToken);

                if (existingItem == null)
                    throw new Exception("Item not found or version mismatch");

                // Обновляем скалярные свойства
                _db.Entry(existingItem).CurrentValues.SetValues(item);
                existingItem.UpdatedAt = DateTime.UtcNow;

                // Обрабатываем изменения в коллекции FieldValues
                await UpdateItemFieldValuesOptimizedAsync(existingItem, item.FieldValues, cancellationToken);

                await _db.SaveChangesAsync(cancellationToken);
                return existingItem;
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new DbUpdateConcurrencyException("Item was modified by another user. Please refresh and try again.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Метод для получения Item с FieldValues
        public async Task<Item> GetByIdWithFieldValuesAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _db.Set<Item>()
                .Include(i => i.FieldValues)
                .Include(i => i.Inventory)
                    .ThenInclude(inv => inv.Fields)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        // Метод для массового обновления Items
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
