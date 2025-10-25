using AutoMapper;
using Main.Application.Dtos.Inventories.Index;
using Main.Application.Dtos.Items.Create;
using Main.Application.Dtos.Items.Index;
using Main.Application.Interfaces;
using Main.Domain.entities.inventory;
using Main.Domain.entities.item;
using Main.Domain.enums.inventory;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Main.Application.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IInventoryService _inventoryService;
        private readonly ICustomIdService _customIdService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ItemService(
            IItemRepository itemRepository,
            IInventoryService inventoryService,
            IInventoryFieldRepository inventoryFieldRepository,
            ICustomIdService customIdService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _itemRepository = itemRepository;
            _inventoryService = inventoryService;
            _customIdService = customIdService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ItemDto> CreateAsync(CreateItemDto createDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (!await _inventoryService.HasWriteAccessAsync(createDto.InventoryId, cancellationToken))
                    throw new UnauthorizedAccessException("Нет прав на добавление предметов в этот инвентарь");

                var fieldSchema = await _inventoryService.GetInventoryFields(createDto.InventoryId, cancellationToken);

                ValidateFieldValues(createDto.FieldValues, fieldSchema);

                // 5. Создаем Item
                var item = new Item
                {
                    InventoryId = createDto.InventoryId,
                    CustomId = createDto.CustomId,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var fieldSchema1 = _mapper.Map<List<InventoryField>>(fieldSchema);
                // 6. Добавляем значения полей
                await AddFieldValuesAsync(item, createDto.FieldValues, fieldSchema1, cancellationToken);

                // 7. Сохраняем
                var createdItem = await _itemRepository.CreateAsync(item, cancellationToken);
                return _mapper.Map<ItemDto>(createdItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteItemAsync(int[] ids, CancellationToken cancellationToken = default)
        {
            var inventory = await _itemRepository.GetFirstAsync(i => i.Id == ids[0]);
            await _itemRepository.DeleteAsync(i => ids.Contains(i.Id), cancellationToken);

            return inventory.InventoryId;
        }

        private void ValidateFieldValues(List<CreateItemFieldValueDto> fieldValues, IEnumerable<InventoryFieldDto> fieldSchema)
        {
            // Проверяем обязательные поля
            var requiredFields = fieldSchema.ToList();
            ValidateAndConvertFieldValues(fieldValues, requiredFields);

        }

        private void ValidateAndConvertFieldValues(List<CreateItemFieldValueDto> fieldValues, List<InventoryFieldDto> fieldSchema)
        {
            var result = new Dictionary<int, object>();
            foreach (var fieldValue in fieldValues)
            {
                var field = fieldSchema.FirstOrDefault(f => f.Id == fieldValue.InventoryFieldId);
                if (field == null)
                    throw new ArgumentException($"Поле с ID {fieldValue.InventoryFieldId} не существует");

                object value = field.FieldType switch
                {
                    FieldType.Text => fieldValue.TextValue,
                    FieldType.MultilineText => fieldValue.MultilineTextValue,
                    FieldType.Number => fieldValue.NumberValue,
                    FieldType.File => fieldValue.FileUrl,
                    FieldType.Boolean => fieldValue.BooleanValue,
                    _ => null
                };

                if (value == null && field.IsRequired)
                    throw new ArgumentException($"Обязательное поле '{field.Name}' не заполнено");

                result[fieldValue.InventoryFieldId] = value;
            }

            // Проверяем все обязательные поля

        }
        private async Task AddFieldValuesAsync(Item item, List<CreateItemFieldValueDto> fieldValues, List<InventoryField> fieldSchema, CancellationToken cancellationToken)
        {
            foreach (var fieldValue in fieldValues)
            {
                var field = fieldSchema.First(f => f.Id == fieldValue.InventoryFieldId);

                var itemFieldValue = new ItemFieldValue
                {
                    InventoryFieldId = field.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                switch (field.FieldType)
                {
                    case FieldType.Text:
                        itemFieldValue.TextValue = fieldValue.TextValue;
                        break;
                    case FieldType.MultilineText:
                        itemFieldValue.MultilineTextValue = fieldValue.MultilineTextValue;
                        break;
                    case FieldType.Number:
                        itemFieldValue.NumberValue = fieldValue.NumberValue;
                        break;
                    case FieldType.File:
                        itemFieldValue.FileUrl = fieldValue.FileUrl;
                        break;
                    case FieldType.Boolean:
                        itemFieldValue.BooleanValue = (bool)fieldValue.BooleanValue;
                        break;
                }

                item.FieldValues.Add(itemFieldValue);
            }
        }

        public async Task<ItemDto> UpdateItemAsync(ItemDto itemDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var item = _mapper.Map<Item>(itemDto);

                var result = await _itemRepository.UpdateItemAsync(item, cancellationToken);

                var resultDto = _mapper.Map<ItemDto>(result);

                return resultDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ItemDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _itemRepository.GetFirstAsync(i => i.Id == id, "FieldValues", cancellationToken);

            return _mapper.Map<ItemDto>(item);
        }

        public async Task<IEnumerable<ItemDto>> GetByInventoryAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _itemRepository.GetAllAsync(i => i.InventoryId == id, "FieldValues", cancellationToken);
            return _mapper.Map<IEnumerable<ItemDto>>(item);
        }

        public async Task<IEnumerable<ItemDto>> GetAllValueAsync(CancellationToken cancellationToken = default)
        {
            var item = await _itemRepository.GetAllAsync(null, "FieldValues", cancellationToken);
            return _mapper.Map<IEnumerable<ItemDto>>(item);
        }

        public async Task<bool> Delete(List<int> ids, CancellationToken cancellationToken)
        {
            await _itemRepository.DeleteAsync(i => ids.Contains(i.Id), cancellationToken);

            return true;
        }

        public async Task<InventoryStatsDto> GetInventoryStatsAsync(int inventoryId)
        {
            try
            {
                var inventory = await _inventoryService.GetById(inventoryId);
                var items = await GetByInventoryAsync(inventoryId);

                var stats = new InventoryStatsDto
                {
                    InventoryId = inventoryId,
                    InventoryName = inventory.Name,
                    TotalItems = items.Count(),
                    TotalFields = inventory.Fields.Count,
                    OldestItemDate = items.Min(i => i.CreatedAt),
                    NewestItemDate = items.Max(i => i.CreatedAt),
                    GeneratedAt = DateTime.UtcNow
                };

                // Статистика по типам полей
                stats.FieldTypeDistribution = CalculateFieldTypeDistribution(inventory.Fields);

                // Детальная статистика
                stats.NumericFieldsStats = await CalculateNumericFieldStatsAsync(items.ToList(), inventory.Fields);
                stats.TextFieldsStats = await CalculateTextFieldStatsAsync(items.ToList(), inventory.Fields);
                stats.BooleanFieldsStats = await CalculateBooleanFieldStatsAsync(items.ToList(), inventory.Fields);

                // Активность
                stats.ActivityStats = CalculateActivityStats(items.ToList());

                // Популярные теги
                //stats.TopTags = CalculateTopTags(items.ToList());

                return stats;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private FieldTypeDistribution CalculateFieldTypeDistribution(ICollection<InventoryFieldDto> fields)
        {
            return new FieldTypeDistribution
            {
                NumericFieldsCount = fields.Count(f => f.FieldType == FieldType.Number),
                TextFieldsCount = fields.Count(f => f.FieldType == FieldType.Text),
                BooleanFieldsCount = fields.Count(f => f.FieldType == FieldType.Boolean),
                FileFieldsCount = fields.Count(f => f.FieldType == FieldType.File),
                MultilineTextFieldsCount = fields.Count(f => f.FieldType == FieldType.MultilineText)
            };
        }

        private async Task<List<NumericFieldStats>> CalculateNumericFieldStatsAsync(
            List<ItemDto> items, ICollection<InventoryFieldDto> fields)
        {
            var numericFields = fields.Where(f => f.FieldType == FieldType.Number).ToList();
            var stats = new List<NumericFieldStats>();

            foreach (var field in numericFields)
            {
                var fieldValues = items
                    .SelectMany(i => i.FieldValues)
                    .Where(fv => fv.InventoryFieldId == field.Id && fv.NumberValue.HasValue)
                    .Select(fv => fv.NumberValue.Value)
                    .ToList();

                if (!fieldValues.Any()) continue;

                var values = fieldValues.Select(v => (double)v).ToList();
                values.Sort();

                var fieldStats = new NumericFieldStats
                {
                    FieldName = field.Name,
                    FieldId = field.Id,
                    Count = fieldValues.Count,
                    Average = fieldValues.Average(),
                    Min = fieldValues.Min(),
                    Max = fieldValues.Max(),
                    Sum = fieldValues.Sum(),
                    Median = CalculateMedian(values),
                    StandardDeviation = CalculateStandardDeviation(values),
                    ValueDistribution = CalculateNumericValueDistribution(fieldValues, 10), // 10 bins
                    Percentiles = CalculatePercentiles(values, [0.25, 0.5, 0.75, 0.9, 0.95])
                };

                stats.Add(fieldStats);
            }

            return stats;
        }

        private async Task<List<TextFieldStats>> CalculateTextFieldStatsAsync(
            List<ItemDto> items, ICollection<InventoryFieldDto> fields)
        {
            var textFields = fields.Where(f => f.FieldType == FieldType.Text || f.FieldType == FieldType.MultilineText).ToList();
            var stats = new List<TextFieldStats>();

            foreach (var field in textFields)
            {
                var fieldValues = items
                    .SelectMany(i => i.FieldValues)
                    .Where(fv => fv.InventoryFieldId == field.Id)
                    .Select(fv => fv.TextValue ?? fv.MultilineTextValue)
                    .Where(value => !string.IsNullOrEmpty(value))
                    .ToList();

                var allValues = items
                    .SelectMany(i => i.FieldValues)
                    .Where(fv => fv.InventoryFieldId == field.Id)
                    .ToList();

                var valueFrequencies = fieldValues
                    .GroupBy(v => v)
                    .Select(g => new ValueFrequency
                    {
                        Value = g.Key ?? string.Empty,
                        Count = g.Count(),
                        Percentage = (double)g.Count() / allValues.Count * 100
                    })
                    .OrderByDescending(vf => vf.Count)
                    .Take(10)
                    .ToList();

                var fieldStats = new TextFieldStats
                {
                    FieldName = field.Name,
                    FieldId = field.Id,
                    TotalValues = allValues.Count,
                    UniqueValues = fieldValues.Distinct().Count(),
                    EmptyValues = allValues.Count(v => string.IsNullOrEmpty(v.TextValue) && string.IsNullOrEmpty(v.MultilineTextValue)),
                    TopValues = valueFrequencies,
                    AverageLength = fieldValues.Any() ? fieldValues.Average(v => v?.Length ?? 0) : 0,
                    MaxLength = fieldValues.Any() ? fieldValues.Max(v => v?.Length ?? 0) : 0,
                    MinLength = fieldValues.Any() ? fieldValues.Min(v => v?.Length ?? 0) : 0
                };

                stats.Add(fieldStats);
            }

            return stats;
        }

        private async Task<List<BooleanFieldStats>> CalculateBooleanFieldStatsAsync(
            List<ItemDto> items, ICollection<InventoryFieldDto> fields)
        {
            var booleanFields = fields.Where(f => f.FieldType == FieldType.Boolean).ToList();
            var stats = new List<BooleanFieldStats>();

            foreach (var field in booleanFields)
            {
                var fieldValues = items
                    .SelectMany(i => i.FieldValues)
                    .Where(fv => fv.InventoryFieldId == field.Id && fv.BooleanValue.HasValue)
                    .Select(fv => fv.BooleanValue.Value)
                    .ToList();

                var fieldStats = new BooleanFieldStats
                {
                    FieldName = field.Name,
                    FieldId = field.Id,
                    TrueCount = fieldValues.Count(v => v),
                    FalseCount = fieldValues.Count(v => !v),
                    TotalCount = fieldValues.Count
                };

                stats.Add(fieldStats);
            }

            return stats;
        }

        // Вспомогательные методы для расчетов
        private double CalculateMedian(List<double> values)
        {
            var count = values.Count;
            if (count == 0) return 0;

            values.Sort();

            if (count % 2 == 0)
                return (values[count / 2 - 1] + values[count / 2]) / 2.0;
            else
                return values[count / 2];
        }

        private double CalculateStandardDeviation(List<double> values)
        {
            var avg = values.Average();
            var sum = values.Sum(v => Math.Pow(v - avg, 2));
            return Math.Sqrt(sum / values.Count);
        }

        private Dictionary<string, int> CalculateNumericValueDistribution(List<double> values, int bins)
        {
            if (!values.Any()) return new Dictionary<string, int>();

            var min = (double)values.Min();
            var max = (double)values.Max();
            var binWidth = (max - min) / bins;

            var distribution = new Dictionary<string, int>();

            for (int i = 0; i < bins; i++)
            {
                var lowerBound = min + i * binWidth;
                var upperBound = min + (i + 1) * binWidth;
                var count = values.Count(v => (double)v >= lowerBound && (double)v < upperBound);

                var rangeLabel = $"{lowerBound:F2} - {upperBound:F2}";
                distribution[rangeLabel] = count;
            }

            return distribution;
        }

        private List<double> CalculatePercentiles(List<double> values, double[] percentiles)
        {
            if (!values.Any()) return new List<double>();

            values.Sort();
            var result = new List<double>();

            foreach (var percentile in percentiles)
            {
                var index = percentile * (values.Count - 1);
                var lowerIndex = (int)Math.Floor(index);
                var upperIndex = (int)Math.Ceiling(index);

                if (lowerIndex == upperIndex)
                {
                    result.Add(values[lowerIndex]);
                }
                else
                {
                    var weight = index - lowerIndex;
                    var value = (1 - weight) * values[lowerIndex] + weight * values[upperIndex];
                    result.Add(value);
                }
            }

            return result;
        }

        private ActivityStats CalculateActivityStats(List<ItemDto> items)
        {
            var now = DateTime.UtcNow;

            return new ActivityStats
            {
                ItemsCreatedLast7Days = items.Count(i => i.CreatedAt >= now.AddDays(-7)),
                ItemsCreatedLast30Days = items.Count(i => i.CreatedAt >= now.AddDays(-30)),
                ItemsUpdatedLast7Days = items.Count(i => i.UpdatedAt >= now.AddDays(-7))
            };
        }

        //private List<ValueFrequency> CalculateTopTags(List<ItemDto> items)
        //{
        //    return items
        //        .SelectMany(i => i.Tags)
        //        .GroupBy(t => t.Name)
        //        .Select(g => new ValueFrequency
        //        {
        //            Value = g.Key,
        //            Count = g.Count(),
        //            Percentage = (double)g.Count() / items.Count * 100
        //        })
        //        .OrderByDescending(t => t.Count)
        //        .Take(10)
        //        .ToList();
        //}

        public async Task<List<NumericFieldStats>> GetNumericFieldStatsAsync(int inventoryId)
        {
            var items = await GetByInventoryAsync(inventoryId);
            var inventory = await _inventoryService.GetById(inventoryId);
            return await CalculateNumericFieldStatsAsync(items.ToList(), inventory.Fields);
        }

        public async Task<List<TextFieldStats>> GetTextFieldStatsAsync(int inventoryId)
        {
            var items = await GetByInventoryAsync(inventoryId);
            var inventory = await _inventoryService.GetById(inventoryId);
            return await CalculateTextFieldStatsAsync(items.ToList(), inventory.Fields);
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
