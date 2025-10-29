using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _db;
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

        public async Task<Inventory> UpdateInventoryAsync(Inventory inventory, CancellationToken cancellationToken = default)
        {
            try
            {
                // Загружаем существующую сущность с включением Fields
                var existingInventory = await _db.Set<Inventory>()
                    .Include(i => i.Fields)
                    .FirstOrDefaultAsync(i => i.Id == inventory.Id, cancellationToken);

                if (existingInventory == null)
                    throw new Exception("Inventory not found");

                // Обновляем скалярные свойства
                _db.Entry(existingInventory).CurrentValues.SetValues(inventory);
                existingInventory.UpdatedAt = DateTime.UtcNow;

                // Обрабатываем изменения в коллекции Fields
                await UpdateInventoryFieldsOptimizedAsync(existingInventory, inventory.Fields, cancellationToken);

                // ✅ ВСЕ изменения сохраняются ОДНИМ запросом
                await _db.SaveChangesAsync(cancellationToken);
                return existingInventory;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task UpdateInventoryFieldsOptimizedAsync(Inventory existingInventory, ICollection<InventoryField> newFields, CancellationToken cancellationToken)
        {
            var existingFieldsDict = existingInventory.Fields.ToDictionary(f => f.Id);
            var newFieldsDict = newFields.Where(f => f.Id != 0).ToDictionary(f => f.Id);

            // 1. Удаляем поля - ОДНА операция для всех
            var fieldsToRemove = existingInventory.Fields
                .Where(existingField => !newFieldsDict.ContainsKey(existingField.Id))
                .ToList();

            if (fieldsToRemove.Any())
            {
                _db.Set<InventoryField>().RemoveRange(fieldsToRemove); // ✅ Bulk remove
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    existingInventory.Fields.Remove(fieldToRemove);
                }
            }

            // 2. Обновляем существующие поля - EF Core отслеживает изменения автоматически
            foreach (var existingField in existingInventory.Fields.Where(f => newFieldsDict.ContainsKey(f.Id)))
            {
                if (newFieldsDict.TryGetValue(existingField.Id, out var newField))
                {
                    _db.Entry(existingField).CurrentValues.SetValues(newField);
                }
            }

            // 3. Добавляем новые поля - ОДНА операция для всех
            var fieldsToAdd = newFields.Where(f => f.Id == 0).ToList();
            if (fieldsToAdd.Any())
            {
                foreach (var fieldToAdd in fieldsToAdd)
                {
                    fieldToAdd.InventoryId = existingInventory.Id;
                    fieldToAdd.CreatedAt = DateTime.UtcNow;
                }
                await _db.Set<InventoryField>().AddRangeAsync(fieldsToAdd, cancellationToken); // ✅ Bulk add
                foreach (var fieldToAdd in fieldsToAdd)
                {
                    existingInventory.Fields.Add(fieldToAdd);
                }
            }
        }
    }
}
