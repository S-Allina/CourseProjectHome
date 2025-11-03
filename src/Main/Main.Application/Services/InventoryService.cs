using AutoMapper;
using FluentValidation;
using Main.Application.Dtos;
using Main.Application.Dtos.Inventories.Create;
using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Main.Application.Interfaces.ImgBBStorage;
using Main.Domain.entities.common;
using Main.Domain.entities.inventory;
using Main.Domain.enums.Users;
using Main.Domain.InterfacesRepository;
using Main.Presentation.MVC.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Main.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryFieldRepository _inventoryFieldRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUsersService _usersService;
        private readonly IValidator<CreateInventoryDto> _fluentValidator;
        private readonly IMapper _mapper;
        private readonly IImgBBStorageService _imgBBStorageService;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            ICategoryRepository categoryRepository,
            IInventoryFieldRepository inventoryFieldRepository,
            IValidator<CreateInventoryDto> fluentValidator,
            IUsersService usersService,
            IImgBBStorageService imgBBStorageService,
        IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryFieldRepository = inventoryFieldRepository;
            _categoryRepository = categoryRepository;
            _fluentValidator = fluentValidator;
            _mapper = mapper;
            _imgBBStorageService = imgBBStorageService;
            _usersService = usersService;
        }

        public async Task<IEnumerable<InventoryTableDto>> GetAll(CancellationToken cancellationToken = default)
        {
            var inventories = await _inventoryRepository.GetAllAsync(null,cancellationToken, "Owner", "Category");
            return _mapper.Map<IEnumerable<InventoryTableDto>>(inventories);
        }

        public async Task<IEnumerable<InventoryTableDto>> GetUserInventoriesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _usersService.GetCurrentUserId();
            var inventories = await _inventoryRepository.GetAllAsync(i => i.OwnerId == userId, cancellationToken, "Owner", "Category", "Fields");
            return _mapper.Map<IEnumerable<InventoryTableDto>>(inventories);
        }

        public async Task<IEnumerable<InventoryTableDto>> GetSharedInventoriesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _usersService.GetCurrentUserId();

            var inventories = await _inventoryRepository.GetAllAsync(i => i.AccessList.Any(a => a.UserId == userId && (int)a.AccessLevel >= 2), cancellationToken, "Owner", "Category", "Fields");
            return _mapper.Map<IEnumerable<InventoryTableDto>>(inventories);
        }

        public async Task<InventoryDetailsDto> GetById(int id, CancellationToken cancellationToken = default)
        {
            var inventories = await _inventoryRepository.GetFirstAsync(i => i.Id == id, cancellationToken, "Fields","AccessList", "Owner", "Category");

            return _mapper.Map<InventoryDetailsDto>(inventories);
        }

        public async Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default)
        {
            var fields = await _inventoryFieldRepository.GetAllAsync(f => f.InventoryId == id, cancellationToken);

            return _mapper.Map<IEnumerable<InventoryFieldDto>>(fields);
        }

        public async Task<InventoryDetailsDto> CreateInventoryAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        {
            var fluentValidationResult = await _fluentValidator.ValidateAsync(createDto, cancellationToken);
            if (!fluentValidationResult.IsValid)
                throw new ValidationException(fluentValidationResult.Errors);

            if (createDto.Image != null)
                createDto.ImageUrl = await _imgBBStorageService.UploadFileAsync(createDto.Image);

            var inventory = _mapper.Map<Inventory>(createDto);

            inventory.OwnerId = _usersService.GetCurrentUserId();
            await AddFieldsToInventory(inventory, createDto.Fields);
            await AddInventoryAccessToInventory(inventory, createDto.AccessList);
            var createdInventory = await _inventoryRepository.CreateAsync(inventory, cancellationToken);

            return _mapper.Map<InventoryDetailsDto>(createdInventory);
        }

        public async Task<IEnumerable<Category>> GetCategories(CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetAllAsync(null, cancellationToken);
        }

        public async Task<bool> DeleteInventoryAsync(int[] ids, CancellationToken cancellationToken = default)
        {
            await _inventoryRepository.DeleteAsync(i => ids.Contains(i.Id), cancellationToken);

            return true;
        }

        public async Task<InventoryDetailsDto> UpdateInventoryAsync(InventoryDetailsDto inventoryDto, CancellationToken cancellationToken = default)
        {
                if (inventoryDto.Image != null)
                    inventoryDto.ImageUrl = await _imgBBStorageService.UploadFileAsync(inventoryDto.Image);

                var inventory = _mapper.Map<Inventory>(inventoryDto);

                var result = await _inventoryRepository.UpdateInventoryAsync(inventory, cancellationToken);

                var resultDto = _mapper.Map<InventoryDetailsDto>(result);

                return resultDto;
        }
        public async Task<InventoryFormViewModel> GetCreateViewModelAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            var viewModel = new InventoryFormViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),

                // Инициализация пустых коллекций
                Tags = new List<string>(),
                AccessList = new List<InventoryAccessDto>(),
                Fields = new List<CreateInventoryFieldDto>()
            };

            return viewModel;
        }

        public async Task<InventoryFormViewModel> GetEditViewModelAsync(int id)
        {
            var inventory = await _inventoryRepository.GetFirstAsync(
                i => i.Id == id,
                cancellationToken: default,
                "Fields", "AccessList", "Category", "Tags");

            if (inventory == null)
                throw new ArgumentException($"Inventory with id {id} not found");

            var categories = await _categoryRepository.GetAllAsync();

            // Сначала маппим в InventoryFormDto, затем создаем ViewModel
            var formDto = _mapper.Map<InventoryFormDto>(inventory);

            var viewModel = new InventoryFormViewModel
            {
                // Копируем все свойства из DTO
                Id = formDto.Id,
                Name = formDto.Name,
                Description = formDto.Description,
                CategoryId = formDto.CategoryId,
                CategoryName = formDto.CategoryName,
                ImageUrl = formDto.ImageUrl,
                Image = formDto.Image,
                IsPublic = formDto.IsPublic,
                CustomIdFormat = formDto.CustomIdFormat,
                OwnerId = formDto.OwnerId,
                CreatedAt = formDto.CreatedAt,
                Version = formDto.Version,
                Tags = formDto.Tags,
                AccessList = formDto.AccessList,
                Fields = formDto.Fields,

                // Заполняем специфичные для ViewModel свойства
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == inventory.CategoryId
                }).ToList()
            };

            return viewModel;
        }
        public async Task<bool> HasWriteAccessAsync(int inventoryId, AccessLevel accessLevel, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryRepository.GetFirstAsync(i => i.Id == inventoryId, cancellationToken, "AccessList");

            if (inventory == null)
                throw new ArgumentException("Инвентарь не найден");

            var userId = _usersService.GetCurrentUserId();

            return inventory.OwnerId == userId ||
                   inventory.IsPublic ||
                   inventory.AccessList.Any(a => a.UserId == userId && (int)a.AccessLevel >= (int)accessLevel);
        }

        public async Task<List<InventorySearchResult>> GetInventoriesByTagAsync(string tagName)
        {
            return null;
        }
        public async Task<IEnumerable<InventoryTableDto>> GetRecentInventoriesAsync(int count, CancellationToken cancellationToken = default)
        {
            var inventories = await _inventoryRepository.GetAllAsync(
                i => i.IsPublic || i.AccessList.Any(a => a.UserId == _usersService.GetCurrentUserId()),
                cancellationToken, "Fields", "Owner"
            );

            var recentInventories = inventories.OrderByDescending(i => i.CreatedAt).Take(count).ToList();

            return _mapper.Map<IEnumerable<InventoryTableDto>>(recentInventories);
        }

        public async Task<IEnumerable<InventoryTableDto>> GetPopularInventoriesAsync(int count, CancellationToken cancellationToken = default)
        {
            var inventoriesWithItemCount = await _inventoryRepository.GetAllAsync(null,  cancellationToken, "Items", "Owner");
            var inventoriesWithItemCountDto = inventoriesWithItemCount.Select(i => new InventoryTableDto
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                OwnerId = i.OwnerId,
                ImageUrl = i.ImageUrl,
                IsPublic = i.IsPublic,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                ItemsCount = i.Items.Count 
            });

            var userId = _usersService.GetCurrentUserId();
            
            var popularInventories = inventoriesWithItemCountDto
                .OrderByDescending(i => i.ItemsCount)
                .Take(count)
                .ToList();

            return (popularInventories);
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

            var userId = _usersService.GetCurrentUserId();
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
    }
}
