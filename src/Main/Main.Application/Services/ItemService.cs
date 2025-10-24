using AutoMapper;
using Main.Application.Dtos;
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
                    FieldType.File => fieldValue.FileValue,
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
                        itemFieldValue.NumberValue = Convert.ToDecimal(fieldValue.NumberValue);
                        break;
                    case FieldType.File:
                        itemFieldValue.FileUrl = fieldValue.FileValue;
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

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
