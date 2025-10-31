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

        [HttpGet]
        [ProducesResponseType(typeof(InventoryStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryStatsDto>> GetInventoryStats(int inventoryId)
        {
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
                return Ok(stats);
        }

        [HttpGet("numeric")]
        [ProducesResponseType(typeof(List<NumericFieldStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<NumericFieldStatsDto>>> GetNumericFieldStats(int inventoryId)
        {
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetNumericFieldStatsAsync(inventoryId);
                return Ok(stats);
        }

        [HttpGet("text")]
        [ProducesResponseType(typeof(List<TextFieldStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TextFieldStatsDto>>> GetTextFieldStats(int inventoryId)
        {
                var inventory = await _inventoryService.GetById(inventoryId);
                if (inventory == null)
                {
                    return NotFound($"Инвентарь с ID {inventoryId} не найден");
                }

                var stats = await _statsService.GetTextFieldStatsAsync(inventoryId);
                return Ok(stats);
        }

        [HttpGet("fields/{fieldId}")]
        [ProducesResponseType(typeof(FieldStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FieldStatsDto>> GetFieldStats(int inventoryId, int fieldId)
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
    }
}
