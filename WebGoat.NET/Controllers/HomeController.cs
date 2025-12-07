using WebGoatCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebGoatCore.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebGoatCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductRepository _productRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ProductRepository productRepository, ILogger<HomeController> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                TopProducts = await _productRepository.GetTopProductsAsync(4)
            };

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult About() => View();

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Admin() => View();

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionInfo = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            if (exceptionInfo != null)
            {
                // Log med så meget detaljeret info som muligt
                _logger.LogError(exceptionInfo.Error,
                    "Unhandled exception caught by Error() action. Path: {Path}, RequestId: {RequestId}",
                    exceptionInfo.Path,
                    requestId);
            }
            else
            {
                // Log fallback
                _logger.LogWarning("Error() invoked but no exception information was available. RequestId: {RequestId}", requestId);
            }

            var model = new ErrorViewModel
            {
                RequestId = requestId,
                ExceptionInfo = exceptionInfo
            };

            return View(model);
        }
    }
}