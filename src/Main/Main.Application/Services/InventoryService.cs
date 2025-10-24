using AutoMapper;
using FluentValidation;
using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Domain.entities.inventory;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Main.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryFieldRepository _inventoryFieldRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IValidator<CreateInventoryDto> _fluentValidator;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            ICategoryRepository categoryRepository,
            IInventoryFieldRepository inventoryFieldRepository,
            IValidator<CreateInventoryDto> fluentValidator,
        IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryFieldRepository = inventoryFieldRepository;
            _categoryRepository = categoryRepository;
            _fluentValidator = fluentValidator;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<InventoryDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var inventories = await _inventoryRepository.GetAllAsync(null, "Fields", cancellationToken);
            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<InventoryDto> GetById(int id, CancellationToken cancellationToken = default)
        {
            var inventories = await _inventoryRepository.GetFirstAsync(i => i.Id == id, "Fields", cancellationToken);

            return _mapper.Map<InventoryDto>(inventories);
        }

        public async Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default)
        {
            var fields = await _inventoryFieldRepository.GetAllAsync(f => f.InventoryId == id, null, cancellationToken);

            return _mapper.Map<IEnumerable<InventoryFieldDto>>(fields);
        }

        public async Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        {
            var fluentValidationResult = await _fluentValidator.ValidateAsync(createDto, cancellationToken);
            if (!fluentValidationResult.IsValid)
            {
                throw new ValidationException(fluentValidationResult.Errors);
            }
            var inventory = _mapper.Map<Inventory>(createDto);

            inventory.OwnerId = GetCurrentUserId();
            await AddFieldsToInventory(inventory, createDto.Fields);
            await AddInventoryAccessToInventory(inventory, createDto.AccessList);
            var createdInventory = await _inventoryRepository.CreateAsync(inventory, cancellationToken);

            return _mapper.Map<InventoryDto>(createdInventory);
        }

        public async Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllAsync(null, null, cancellationToken);
        }

        public async Task<bool> DeleteInventoryAsync(int[] ids, CancellationToken cancellationToken = default)
        {
            await _inventoryRepository.DeleteAsync(i => ids.Contains(i.Id), cancellationToken);

            return true;
        }

        public async Task<InventoryDto> UpdateInventoryAsync(InventoryDto inventoryDto, CancellationToken cancellationToken = default)
        {
            try
            {
                var inventory = _mapper.Map<Inventory>(inventoryDto);

                var result = await _inventoryRepository.UpdateInventoryAsync(inventory, cancellationToken);

                var resultDto = _mapper.Map<InventoryDto>(result);

                return resultDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> HasWriteAccessAsync(int inventoryId, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryRepository.GetFirstAsync(i => i.Id == inventoryId, "AccessList", cancellationToken);

            if (inventory == null)
                throw new ArgumentException("Инвентарь не найден");

            var userId = GetCurrentUserId();

            return inventory.OwnerId == userId ||
                   inventory.IsPublic ||
                   inventory.AccessList.Any(a => a.UserId == userId && (int)a.AccessLevel >= 2);
        }

        public async Task<List<InventorySearchResult>> GetInventoriesByTagAsync(string tagName)
        {
            return null;
        }

        private async Task AddFieldsToInventory(Inventory inventory, List<CreateInventoryFieldDto> fieldDtos)
        {
            if (!fieldDtos.Any()) return;

            foreach (var fieldDto in fieldDtos.OrderBy(f => f.OrderIndex))
            {
                inventory.Fields.Add(new InventoryField
                {
                    Name = fieldDto.Name.Trim(),
                    Description = fieldDto.Description?.Trim(),
                    FieldType = fieldDto.FieldType,
                    OrderIndex = fieldDto.OrderIndex,
                    IsVisibleInTable = fieldDto.IsVisibleInTable,
                    IsRequired = fieldDto.IsRequired,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        private async Task AddInventoryAccessToInventory(Inventory inventory, List<CreateInventoryAccessDto> accessDtos)
        {
            if (!accessDtos.Any()) return;

            var userId = GetCurrentUserId();
            foreach (var dto in accessDtos)
            {
                inventory.AccessList.Add(new InventoryAccess
                {
                    AccessLevel = dto.AccessLevel,
                    InventoryId = inventory.Id,
                    UserId = dto.UserId,
                    GrantedById = userId,
                    GrantedAt = DateTime.UtcNow
                });
            }
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
