using Main.Domain.entities;
using Main.Domain.InterfacesRepository;
using Microsoft.EntityFrameworkCore;

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
                var result = await _context.Database
                    .SqlQuery<int>($"SELECT SERVERPROPERTY('IsFullTextInstalled')").FirstOrDefaultAsync(cancellationToken);

                return result == 1;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<UserSearchResult>> SearchUsersAsync(string searchTerm, int limit = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                    return new List<UserSearchResult>();

                var isFullTextAvailable = true;

                if (isFullTextAvailable)
                {
                    // Используем Full-Text Search если доступен
                    return await _context.Users
                        .Where(u => EF.Functions.FreeText(u.Email, searchTerm) ||
                                   EF.Functions.FreeText(u.FirstName, searchTerm) ||
                                   EF.Functions.FreeText(u.LastName, searchTerm))
                        .OrderBy(u => u.FirstName)
                        .ThenBy(u => u.LastName)
                        .Take(limit)
                        .Select(u => new UserSearchResult
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            DisplayName = $"{u.FirstName} {u.LastName}"
                        })
                        .ToListAsync(cancellationToken);
                }
                else
                {
                    // Fallback: используем обычный LIKE с индексами
                    var normalizedSearch = $"{searchTerm}%";
                    return await _context.Users
                        .Where(u => u.Email.StartsWith(normalizedSearch) ||
                                   u.FirstName.StartsWith(normalizedSearch) ||
                                   u.LastName.StartsWith(normalizedSearch))
                        .OrderBy(u => u.FirstName)
                        .ThenBy(u => u.LastName)
                        .Take(limit)
                        .Select(u => new UserSearchResult
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            DisplayName = $"{u.FirstName} {u.LastName}"
                        })
                        .ToListAsync(cancellationToken);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Получение деталей пользователей по ID
        /// </summary>
        public async Task<List<UserSearchResult>> GetUsersDetailsAsync(List<string> userIds, CancellationToken cancellationToken = default)
        {
            if (userIds == null || !userIds.Any())
                return new List<UserSearchResult>();

            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new UserSearchResult
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DisplayName = $"{u.FirstName} {u.LastName}"
                })
                .ToListAsync(cancellationToken);
        }

        // Обновляем GlobalSearchAsync чтобы включить поиск пользователей
        public async Task<GlobalSearchResult> GlobalSearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < 2)
                return new GlobalSearchResult { SearchTerm = searchTerm };

            var result = new GlobalSearchResult { SearchTerm = searchTerm };

            // 🔍 1. Поиск по инвентарям
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

            // 🔍 2. Поиск по значениям полей предметов
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

            // 🔍 3. Поиск по пользователям (НОВОЕ!)
            result.Users = await SearchUsersAsync(searchTerm, 20, cancellationToken);

            return result;
        }

        // Обновляем QuickSearchAsync чтобы включить пользователей
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
                    Id = i.Id.ToString(),
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
                    Id = iv.ItemId.ToString(),
                    Name = $"{iv.InventoryField.Name}: {iv.TextValue ?? iv.MultilineTextValue}",
                    Type = "Item",
                    AdditionalInfo = iv.Item.Inventory.Name,
                    Url = $"/Items/Details/{iv.ItemId}"
                })
                .Take(5)
                .ToListAsync(cancellationToken);

            // ⚡ Быстрый поиск пользователей (НОВОЕ!)
            var userResults = await SearchUsersAsync(searchTerm, 5, cancellationToken);
            var userSearchItems = userResults.Select(u => new QuickSearchItem
            {
                Id = u.Id,
                Name = u.DisplayName,
                Type = "User",
                AdditionalInfo = u.Email,
                Url = "#" // или URL профиля пользователя
            }).ToList();

            result.Results.AddRange(inventoryResults);
            result.Results.AddRange(itemFieldResults);
            result.Results.AddRange(userSearchItems);

            return result;
        }
    }
}
