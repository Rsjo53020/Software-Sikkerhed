using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebGoatCore.ViewModels;

namespace WebGoatCore.Controllers
{
    [AllowAnonymous]
    public class StatusCodeController : Controller
    {
        public const string NAME = "StatusCode";

        private readonly ILogger<StatusCodeController> _logger;

        public StatusCodeController(ILogger<StatusCodeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route(NAME)]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult StatusCodeView([FromQuery] int code)
        {
            var feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            _logger.LogWarning(
                "Status code {StatusCode} for original path {Path}{Query}",
                code,
                feature?.OriginalPath,
                feature?.OriginalQueryString
            );
            
            if (code < 100 || code > 599)
            {
                _logger.LogWarning("Invalid status code {StatusCode} requested. Falling back to 500.", code);
                code = 500;
            }

            var model = StatusCodeViewModel.Create(new ApiResponse(code));

            var viewResult = View(model);
            viewResult.StatusCode = code;

            return viewResult;
        }
    }
}