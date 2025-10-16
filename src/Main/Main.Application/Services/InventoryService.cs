using AutoMapper;
using FluentValidation;
using Main.Application.Dtos;
using Main.Application.Interfaces;
using Main.Domain.entities.inventory;
using Main.Domain.enums.inventory;
using Main.Domain.InterfacesRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Main.Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryFieldRepository _inventoryFieldRepository;
        private readonly IValidator<CreateInventoryDto> _fluentValidator;
        private readonly IMapper _mapper;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IInventoryFieldRepository inventoryFieldRepository,
            IValidator<CreateInventoryDto> fluentValidator,
        IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryFieldRepository = inventoryFieldRepository;
            _fluentValidator= fluentValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InventoryDto>> GetAll(CancellationToken cancellationToken = default)
        { 
               var inventories =  await _inventoryRepository.GetAllAsync(null, "Fields", cancellationToken);
            return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
        }

        public async Task<InventoryDto> GetById(int id, CancellationToken cancellationToken = default)
        {
            var inventories = await _inventoryRepository.GetFirstAsync(i=>i.Id==id, "Fields", cancellationToken);

            return _mapper.Map<InventoryDto>(inventories);
        }

        public async Task<IEnumerable<InventoryFieldDto>> GetInventoryFields(int id, CancellationToken cancellationToken = default)
        {
            var fields = await _inventoryFieldRepository.GetAllAsync(f=>f.InventoryId==id, null, cancellationToken);

            return _mapper.Map<IEnumerable<InventoryFieldDto>>(fields);
        }

        public async Task<InventoryDto> CreateInventoryAsync(CreateInventoryDto createDto, string ownerId, CancellationToken cancellationToken = default)
        {
            var fluentValidationResult = await _fluentValidator.ValidateAsync(createDto, cancellationToken);
            if (!fluentValidationResult.IsValid)
            {
                throw new ValidationException(fluentValidationResult.Errors);
            }
            var inventory = _mapper.Map<Inventory>(createDto);
            inventory.OwnerId = ownerId;
            await AddFieldsToInventory(inventory, createDto.Fields);

            var createdInventory = await _inventoryRepository.CreateAsync(inventory, cancellationToken);

            return _mapper.Map<InventoryDto>(createdInventory);
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
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> HasWriteAccessAsync(int inventoryId, string userId, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryRepository.GetFirstAsync(i => i.Id == inventoryId, "AccessList", cancellationToken);

            if (inventory == null)
                throw new ArgumentException("Инвентарь не найден");

            return inventory.OwnerId == userId ||
                   inventory.IsPublic ||
                   inventory.AccessList.Any(a => a.UserId == userId && a.AccessLevel >= 2);
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
    }
    }
