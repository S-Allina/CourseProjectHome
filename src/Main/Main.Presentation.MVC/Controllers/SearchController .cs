using Main.Domain.entities;
using Main.Domain.InterfacesRepository;
using Microsoft.AspNetCore.Mvc;

namespace Main.Presentation.MVC.Controllers
{
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

        /// <summary>
        /// 🔧 Проверка доступности Full-Text Search
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> CheckAvailability(CancellationToken cancellationToken)
        {
            var isAvailable = await _searchRepository.IsFullTextAvailableAsync(cancellationToken);
            return Json(new { fullTextAvailable = isAvailable });
        }
    }
}
