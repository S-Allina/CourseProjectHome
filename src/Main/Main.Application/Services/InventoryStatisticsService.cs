using Main.Application.Dtos.Inventories.Index;
using Main.Application.Dtos.Items.Index;
using Main.Application.Interfaces;
using Main.Domain.entities.inventory;
using Main.Domain.entities.item;
using Main.Domain.enums.inventory;
using Main.Domain.InterfacesRepository;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Application.Services
{
    public class InventoryStatsService : IInventoryStatsService
    {
        private readonly IItemService _itemService;
        private readonly IInventoryService _inventoryService;

        public InventoryStatsService(IItemService itemService, IInventoryService inventoryService)
        {
            _itemService = itemService;
            _inventoryService = inventoryService;
        }

        public async Task<InventoryStatsDto> GetInventoryStatsAsync(int inventoryId)
        {
            var items = (await _itemService.GetByInventoryAsync(inventoryId)).ToList();
            var inventory = await _inventoryService.GetById(inventoryId);
            var fields = inventory.Fields;

            var fieldStatistics = new List<FieldStatsDto>();

            foreach (var field in fields)
            {
                var fieldValues = items.SelectMany(item => item.FieldValues
                    .Where(fv => fv.InventoryFieldId == field.Id))
                    .ToList();

                var stats = new FieldStatsDto
                {
                    FieldId = field.Id,
                    FieldName = field.Name,
                    FieldType = field.FieldType,
                    EmptyValuesCount = fieldValues.Count(fv => IsEmptyValue(fv, field.FieldType)),
                    NonEmptyValuesCount = fieldValues.Count(fv => !IsEmptyValue(fv, field.FieldType))
                };

                if (field.FieldType == FieldType.Number)
                {
                    var numericValues = fieldValues
                        .Where(fv => fv.NumberValue.HasValue)
                        .Select(fv => fv.NumberValue.Value)
                        .ToList();

                    if (numericValues.Any())
                    {
                        stats.MinValue = numericValues.Min();
                        stats.MaxValue = numericValues.Max();
                        stats.AverageValue = numericValues.Average();
                    }
                }

                if (field.FieldType == FieldType.Text || field.FieldType == FieldType.MultilineText)
                {
                    var textValues = fieldValues
                        .Select(fv => GetTextValue(fv, field.FieldType))
                        .Where(v => !string.IsNullOrEmpty(v))
                        .ToList();

                    stats.ValueCounts = textValues
                        .GroupBy(v => v)
                        .ToDictionary(g => g.Key, g => g.Count());

                    stats.UniqueValuesCount = stats.ValueCounts.Count;
                }

                if (field.FieldType == FieldType.Boolean)
                {
                    var boolValues = fieldValues
                        .Where(fv => fv.BooleanValue.HasValue)
                        .Select(fv => fv?.BooleanValue.Value.ToString())
                        .ToList();

                    stats.ValueCounts = boolValues
                        .GroupBy(v => v)
                        .ToDictionary(g => g.Key, g => g.Count());
                }

                fieldStatistics.Add(stats);
            }

            return new InventoryStatsDto
            {
                TotalItems = items.Count,
                TotalFields = fields.Count,
                OldestItemDate = items.Any() ? items.Min(i => i.CreatedAt) : null,
                NewestItemDate = items.Any() ? items.Max(i => i.CreatedAt) : null,
                FieldStatistics = fieldStatistics
            };
        }

        public async Task<List<NumericFieldStatsDto>> GetNumericFieldStatsAsync(int inventoryId)
        {
            var stats = await GetInventoryStatsAsync(inventoryId);

            return stats.FieldStatistics
                .Where(f => f.FieldType == FieldType.Number && f.NonEmptyValuesCount > 0)
                .Select(f => new NumericFieldStatsDto
                {
                    FieldId = f.FieldId,
                    FieldName = f.FieldName,
                    MinValue = f.MinValue ?? 0,
                    MaxValue = f.MaxValue ?? 0,
                    AverageValue = f.AverageValue ?? 0,
                    ValueDistribution = f.ValueCounts.Select(v => new ValueCountDto
                    {
                        Value = v.Key,
                        Count = v.Value
                    }).ToList()
                })
                .ToList();
        }

        public async Task<List<TextFieldStatsDto>> GetTextFieldStatsAsync(int inventoryId)
        {
            var stats = await GetInventoryStatsAsync(inventoryId);

            return stats.FieldStatistics
                .Where(f => (f.FieldType == FieldType.Text || f.FieldType == FieldType.MultilineText) &&
                           f.ValueCounts.Any())
                .Select(f => new TextFieldStatsDto
                {
                    FieldId = f.FieldId,
                    FieldName = f.FieldName,
                    TopValues = f.ValueCounts
                        .OrderByDescending(v => v.Value)
                        .Take(10)
                        .Select(v => new ValueCountDto { Value = v.Key, Count = v.Value })
                        .ToList(),
                    UniqueValuesCount = f.UniqueValuesCount ?? 0
                })
                .ToList();
        }

        private static bool IsEmptyValue(ItemFieldValueDto fieldValue, FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.Number => !fieldValue.NumberValue.HasValue,
                FieldType.Text => string.IsNullOrEmpty(fieldValue.TextValue),
                FieldType.MultilineText => string.IsNullOrEmpty(fieldValue.MultilineTextValue),
                FieldType.Boolean => !fieldValue.BooleanValue.HasValue,
                FieldType.File => string.IsNullOrEmpty(fieldValue.FileUrl),
                _ => true
            };
        }

        private static string GetTextValue(ItemFieldValueDto fieldValue, FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.Text => fieldValue.TextValue,
                FieldType.MultilineText => fieldValue.MultilineTextValue,
                _ => string.Empty
            };
        }
    }
}
