using Main.Domain.entities;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearchRepository searchRepository, ILogger<SearchController> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        /// <summary>
        /// 🔍 Главная страница полнотекстового поиска
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(string q, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Full-text search executed for term: {SearchTerm}", q);

            ViewBag.SearchTerm = q;

            if (string.IsNullOrWhiteSpace(q))
            {
                return View(new GlobalSearchResult { SearchTerm = q });
            }

            try
            {
                var result = await _searchRepository.GlobalSearchAsync(q, cancellationToken);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during full-text search for term: {SearchTerm}", q);
                ModelState.AddModelError("", "Произошла ошибка при выполнении поиска");
                return View(new GlobalSearchResult { SearchTerm = q });
            }
        }

        /// <summary>
        /// ⚡ Быстрый поиск для автодополнения (JSON API)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> QuickSearch(string term, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new { success = true, data = new QuickSearchResult() });
            }

            try
            {
                var result = await _searchRepository.QuickSearchAsync(term, cancellationToken);
                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during quick search for term: {SearchTerm}", term);
                return Json(new { success = false, error = "Ошибка поиска" });
            }
        }

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Searching users for term: {SearchTerm}", query);

                if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                {
                    return Ok(new List<object>());
                }

                var users = await _searchRepository.SearchUsersAsync(query, limit, cancellationToken);

                var result = users.Select(u => new
                {
                    id = u.Id,
                    email = u.Email,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    displayName = u.DisplayName
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users for term: {SearchTerm}", query);
                return StatusCode(500, new { error = "Произошла ошибка при поиске пользователей" });
            }
        }

        /// <summary>
        /// Получение деталей пользователей по ID (для отображения в таблице доступа)
        /// </summary>
        [HttpPost("users-details")]
        public async Task<IActionResult> GetUsersDetails([FromBody] GetUsersDetailsRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Getting details for {Count} users", request.UserIds?.Count ?? 0);

                if (request.UserIds == null || !request.UserIds.Any())
                {
                    return Ok(new List<object>());
                }

                var users = await _searchRepository.GetUsersDetailsAsync(request.UserIds, cancellationToken);

                var result = users.Select(u => new
                {
                    id = u.Id,
                    email = u.Email,
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    displayName = u.DisplayName
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users details for {Count} users", request.UserIds?.Count ?? 0);
                return StatusCode(500, new { error = "Произошла ошибка при получении данных пользователей" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckAvailability(CancellationToken cancellationToken)
        {
            var isAvailable = await _searchRepository.IsFullTextAvailableAsync(cancellationToken);
            return Json(new { fullTextAvailable = isAvailable });
        }
    }
    public class GetUsersDetailsRequest
    {
        public List<string> UserIds { get; set; } = new();
    }
}
