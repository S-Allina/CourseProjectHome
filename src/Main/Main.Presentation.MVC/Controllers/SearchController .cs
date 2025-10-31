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

        [HttpGet("Index")]
        public async Task<IActionResult> Index(string q, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Full-text search executed for term: {SearchTerm}", q);

            ViewBag.SearchTerm = q;

            if (string.IsNullOrWhiteSpace(q))
            {
                return View(new GlobalSearchResult { SearchTerm = q });
            }

                var result = await _searchRepository.GlobalSearchAsync(q, cancellationToken);
                return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> QuickSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            {
                return Json(new { success = true, data = new QuickSearchResult() });
            }

                var result = await _searchRepository.QuickSearchAsync(term);
                return Json(new { success = true, data = result });
        }

        [HttpGet("CheckAvailability")]
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
