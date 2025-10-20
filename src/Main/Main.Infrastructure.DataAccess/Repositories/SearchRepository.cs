using Main.Domain.entities;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Infrastructure.DataAccess.Repositories
{
    public class SearchRepository : ISearchRepository
    {
        private readonly ApplicationDbContext _context;

        public SearchRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsFullTextAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                // Проверяем доступность Full-Text Search
                var result = await _context.Database
                    .SqlQueryRaw<int>("SELECT SERVERPROPERTY('IsFullTextInstalled')")
                    .FirstOrDefaultAsync(cancellationToken);

                return result == 1;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GlobalSearchResult> GlobalSearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                return new GlobalSearchResult { SearchTerm = searchTerm };

            var result = new GlobalSearchResult { SearchTerm = searchTerm };

            // 🔍 1. Поиск по инвентарям с использованием Full-Text
            result.Inventories = await _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.Items)
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm) ||
                           EF.Functions.FreeText(i.Description, searchTerm))
                .Select(i => new InventorySearchResult
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    CategoryName = i.Category != null ? i.Category.Name : "Без категории",
                    CreatedAt = i.CreatedAt,
                    ItemsCount = i.Items.Count
                })
                .OrderBy(i => i.Name)
                .Take(100)
                .ToListAsync(cancellationToken);

            // 🔍 2. Поиск по значениям полей предметов с использованием Full-Text
            result.ItemFields = await _context.ItemFieldValues
                .Include(iv => iv.Item)
                    .ThenInclude(item => item.Inventory)
                .Include(iv => iv.InventoryField)
                .Where(iv => EF.Functions.FreeText(iv.TextValue, searchTerm) ||
                            EF.Functions.FreeText(iv.MultilineTextValue, searchTerm))
                .Select(iv => new ItemFieldSearchResult
                {
                    ItemId = iv.ItemId,
                    ItemCustomId = iv.Item.CustomId,
                    InventoryId = iv.Item.InventoryId,
                    InventoryName = iv.Item.Inventory.Name,
                    FieldName = iv.InventoryField.Name,
                    FieldValue = iv.TextValue ?? iv.MultilineTextValue ?? string.Empty,
                    FieldType = iv.InventoryField.FieldType
                })
                .OrderBy(iv => iv.InventoryName)
                .ThenBy(iv => iv.FieldName)
                .Take(100)
                .ToListAsync(cancellationToken);

            // 🔍 3. Поиск по пользователям (опционально)
           

            return result;
        }

        public async Task<QuickSearchResult> QuickSearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                return new QuickSearchResult { SearchTerm = searchTerm };

            var result = new QuickSearchResult { SearchTerm = searchTerm };

            // ⚡ Быстрый поиск инвентарей
            var inventoryResults = await _context.Inventories
                .Where(i => EF.Functions.FreeText(i.Name, searchTerm))
                .Select(i => new QuickSearchItem
                {
                    Id = i.Id,
                    Name = i.Name,
                    Type = "Inventory",
                    AdditionalInfo = i.Description.Length > 50
                        ? i.Description.Substring(0, 50) + "..."
                        : i.Description,
                    Url = $"/Inventories/Details/{i.Id}"
                })
                .Take(5)
                .ToListAsync(cancellationToken);

            // ⚡ Быстрый поиск значений полей предметов
            var itemFieldResults = await _context.ItemFieldValues
                .Include(iv => iv.Item)
                    .ThenInclude(item => item.Inventory)
                .Include(iv => iv.InventoryField)
                .Where(iv => EF.Functions.FreeText(iv.TextValue, searchTerm) ||
                            EF.Functions.FreeText(iv.MultilineTextValue, searchTerm))
                .Select(iv => new QuickSearchItem
                {
                    Id = iv.ItemId,
                    Name = $"{iv.InventoryField.Name}: {iv.TextValue ?? iv.MultilineTextValue}",
                    Type = "Item",
                    AdditionalInfo = iv.Item.Inventory.Name,
                    Url = $"/Items/Details/{iv.ItemId}"
                })
                .Take(5)
                .ToListAsync(cancellationToken);

            result.Results.AddRange(inventoryResults);
            result.Results.AddRange(itemFieldResults);

            return result;
        }
    }
}
