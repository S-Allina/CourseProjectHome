using Main.Application.Interfaces;
using Main.Domain.InterfacesRepository;
using System.Text;

namespace Main.Application.Services
{
    public class CustomIdService : ICustomIdService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IItemRepository _itemRepository;

        public CustomIdService(IInventoryRepository inventoryRepository, IItemRepository itemRepository)
        {
            _inventoryRepository = inventoryRepository;
            _itemRepository = itemRepository;
        }

        public async Task<string> GenerateCustomIdAsync(int inventoryId, CancellationToken cancellationToken = default)
        {
            var inventory = await _inventoryRepository.GetFirstAsync(
                i => i.Id == inventoryId, cancellationToken);

            if (inventory == null)
                throw new ArgumentException("Inventory not found");

            if (string.IsNullOrEmpty(inventory.CustomIdFormat))
                return Guid.NewGuid().ToString("N").Substring(0, 8);

            var customId = await GenerateFromStringTemplateAsync(inventory.CustomIdFormat, inventoryId, cancellationToken);

            if (await IsCustomIdExistsAsync(inventoryId, customId, cancellationToken))
                return await GenerateCustomIdAsync(inventoryId, cancellationToken);

            return customId;
        }

        private async Task<string> GenerateFromStringTemplateAsync(string template, int inventoryId, CancellationToken cancellationToken)
        {
            var result = new StringBuilder();
            var index = 0;

            while (index < template.Length)
            {
                if (template[index] == '{')
                {
                    var endIndex = template.IndexOf('}', index);
                    if (endIndex == -1) break;

                    var component = template.Substring(index + 1, endIndex - index - 1);
                    result.Append(await GenerateStringComponentAsync(component, inventoryId, cancellationToken));
                    index = endIndex + 1;
                }
                else
                {
                    result.Append(template[index]);
                    index++;
                }
            }

            return result.ToString();
        }

        private async Task<string> GenerateStringComponentAsync(string component, int inventoryId, CancellationToken cancellationToken)
        {
            if (component.StartsWith("SEQ:"))
            {
                var digits = 6;
                if (component.Length > 4)
                {
                    int.TryParse(component.Substring(4), out digits);
                }
                return await GenerateSequenceComponentAsync(inventoryId, digits, cancellationToken);
            }

            return component.ToUpper() switch
            {
                "R20" => GenerateRandomNumber(0, 1048575).ToString("X5"),
                "R32" => GenerateRandomNumber(0, int.MaxValue - 1).ToString("X8"),
                "R6D" => GenerateRandomNumber(0, 999999).ToString("D6"),
                "R9D" => GenerateRandomNumber(0, 999999999).ToString("D9"),
                "GUID" => Guid.NewGuid().ToString("N").Substring(0, 8),
                "YYYYMMDD" => DateTime.UtcNow.ToString("yyyyMMdd"),
                "YYMMDD" => DateTime.UtcNow.ToString("yyMMdd"),
                "YYYY-MM-DD" => DateTime.UtcNow.ToString("yyyy-MM-dd"),
                "DDMMYYYY" => DateTime.UtcNow.ToString("ddMMyyyy"),
                "UNIX" => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                _ => component
            };
        }

        private async Task<string> GenerateSequenceComponentAsync(int inventoryId, int digits, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetFirstAsync(i => i.Id == inventoryId, cancellationToken);

            if (inventory == null) return "1".PadLeft(digits, '0');

            try
            {
                inventory.CurrentSequence++;
                inventory = await _inventoryRepository.UpdateAsync(inventory);

                return inventory.CurrentSequence.ToString($"D{digits}");
            }
            catch (Exception)
            {
                return await GenerateSequenceComponentAsync(inventoryId, digits, cancellationToken);
            }
        }

        private int GenerateRandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max + 1);
        }

        private async Task<bool> IsCustomIdExistsAsync(int inventoryId, string customId, CancellationToken cancellationToken)
        {
            return await _itemRepository.IsExistsAsync(i => i.InventoryId == inventoryId && i.CustomId == customId, cancellationToken);
        }

        public string GeneratePreview(string template)
        {
            if (string.IsNullOrEmpty(template))
                return "PREVIEW";

            var result = new StringBuilder();
            var index = 0;

            while (index < template.Length)
            {
                if (template[index] == '{')
                {
                    var endIndex = template.IndexOf('}', index);
                    if (endIndex == -1) break;

                    var component = template.Substring(index + 1, endIndex - index - 1);
                    result.Append(GeneratePreviewComponent(component));
                    index = endIndex + 1;
                }
                else
                {
                    result.Append(template[index]);
                    index++;
                }
            }

            return result.ToString();
        }

        private string GeneratePreviewComponent(string component)
        {
            if (component.StartsWith("SEQ:"))
            {
                var digits = 6;
                if (component.Length > 4)
                {
                    int.TryParse(component.Substring(4), out digits);
                }
                return "1".PadLeft(digits, '0');
            }

            return component.ToUpper() switch
            {
                "R20" => "A1B2C",
                "R32" => "A1B2C3D4",
                "R6D" => "123456",
                "R9D" => "123456789",
                "GUID" => "a1b2c3d4",
                "YYYYMMDD" => DateTime.UtcNow.ToString("yyyyMMdd"),
                "YYMMDD" => DateTime.UtcNow.ToString("yyMMdd"),
                "YYYY-MM-DD" => DateTime.UtcNow.ToString("yyyy-MM-dd"),
                "DDMMYYYY" => DateTime.UtcNow.ToString("ddMMyyyy"),
                "UNIX" => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                _ => $"[{component}]"
            };
        }

        public async Task<bool> ValidateCustomIdAsync(int inventoryId, string customId, CancellationToken cancellationToken = default)
        {
            return !await IsCustomIdExistsAsync(inventoryId, customId, cancellationToken);
        }
    }
}
