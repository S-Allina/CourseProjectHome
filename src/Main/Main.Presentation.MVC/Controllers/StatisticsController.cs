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

        public InventoryStatsController(IInventoryStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        public async Task<ActionResult<InventoryStatsDto>> GetInventoryStats(int inventoryId)
        {
            var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
            return Ok(stats);
        }

        [HttpGet("numeric")]
        public async Task<ActionResult<List<NumericFieldStatsDto>>> GetNumericFieldStats(int inventoryId)
        {
            var stats = await _statsService.GetNumericFieldStatsAsync(inventoryId);
            return Ok(stats);
        }

        [HttpGet("text")]
        public async Task<ActionResult<List<TextFieldStatsDto>>> GetTextFieldStats(int inventoryId)
        {
            var stats = await _statsService.GetTextFieldStatsAsync(inventoryId);
            return Ok(stats);
        }

        [HttpGet("fields/{fieldId}")]
        public async Task<ActionResult<FieldStatsDto>> GetFieldStats(int inventoryId, int fieldId)
        {
            var stats = await _statsService.GetInventoryStatsAsync(inventoryId);
            var fieldStats = stats.FieldStatistics.FirstOrDefault(f => f.FieldId == fieldId);
            return Ok(fieldStats);
        }
    }
}
