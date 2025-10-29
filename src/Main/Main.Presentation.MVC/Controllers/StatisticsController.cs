using Main.Application.Dtos.Inventories.Index;
using Main.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
    [ApiController]
    [Route("api/inventories/{inventoryId}/stats")]
    public class InventoryStatsController : ControllerBase
    {
        private readonly IInventoryStatsService _statsService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryStatsController> _logger;

        public InventoryStatsController(
            IInventoryStatsService statsService,
            IInventoryService inventoryService,
            ILogger<InventoryStatsController> logger)
        {
            _statsService = statsService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        /// <summary>
        /// Получить общую статистику инвентаря
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(InventoryStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryStatsDto>> GetInventoryStats(int inventoryId)
        {
            try
            {
                // Проверяем существование инвентаря
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики инвентаря {InventoryId}", inventoryId);
                return StatusCode(500, "Произошла ошибка при получении статистики");
            }
        }

        /// <summary>
        /// Получить статистику числовых полей
        /// </summary>
        [HttpGet("numeric")]
        [ProducesResponseType(typeof(List<NumericFieldStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<NumericFieldStatsDto>>> GetNumericFieldStats(int inventoryId)
        {
            try
            {
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetNumericFieldStatsAsync(inventoryId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики числовых полей инвентаря {InventoryId}", inventoryId);
                return StatusCode(500, "Произошла ошибка при получении статистики числовых полей");
            }
        }

        /// <summary>
        /// Получить статистику текстовых полей
        /// </summary>
        [HttpGet("text")]
        [ProducesResponseType(typeof(List<TextFieldStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TextFieldStatsDto>>> GetTextFieldStats(int inventoryId)
        {
            try
            {
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetTextFieldStatsAsync(inventoryId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики текстовых полей инвентаря {InventoryId}", inventoryId);
                return StatusCode(500, "Произошла ошибка при получении статистики текстовых полей");
            }
        }

        /// <summary>
        /// Получить статистику по конкретному полю
        /// </summary>
        [HttpGet("fields/{fieldId}")]
        [ProducesResponseType(typeof(FieldStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FieldStatsDto>> GetFieldStats(int inventoryId, int fieldId)
        {
            try
            {
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
                var fieldStats = stats.FieldStatistics.FirstOrDefault(f => f.FieldId == fieldId);

                if (fieldStats == null)
                {
                    return NotFound($"Поле с ID {fieldId} не найдено в инвентаре");
                }

                return Ok(fieldStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статистики поля {FieldId} инвентаря {InventoryId}", fieldId, inventoryId);
                return StatusCode(500, "Произошла ошибка при получении статистики поля");
            }
        }
    }
}
