using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebGoatCore.ViewModels;
using System.Linq;

namespace WebGoatCore.Controllers
{
    [Route("[controller]/[action]")]
    public class BlogController : Controller
    {
        private readonly BlogEntryRepository _blogEntryRepository;
        private readonly BlogResponseRepository _blogResponseRepository;

        public BlogController(
            BlogEntryRepository blogEntryRepository,
            BlogResponseRepository blogResponseRepository)
        {
            _blogEntryRepository = blogEntryRepository;
            _blogResponseRepository = blogResponseRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var entries = _blogEntryRepository.GetTopBlogEntries();

            var viewModels = entries.Select(e => new BlogEntryViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Contents = e.Contents,
                PostedDate = e.PostedDate,
                Author = e.Author
            }).ToList();

            return View(viewModels);
        }

        [HttpGet("{entryId:int}")]
        [Authorize]
        public IActionResult Reply(int entryId)
        {
            var responseVm = BuildResponseViewModel(entryId);
            if (responseVm == null)
            {
                TempData["Error"] = "The blog entry you are trying to respond to does not exist.";
                return RedirectToAction(nameof(Index));
            }

            return View(responseVm);
        }

        [HttpPost("{entryId:int}")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Reply(int entryId, BlogResponseViewModel model)
        {
            // Sørg for at model har det rigtige entryId
            model.BlogEntryId = entryId;

            var userName = User?.Identity?.Name ?? "Anonymous";

            // RATE LIMIT: max 5 responses per hour per user
            var responsesLastHour = _blogResponseRepository.CountResponsesByAuthorLastHour(userName);
            if (responsesLastHour >= 5)
            {
                TempData["Error"] = "You have already posted 5 responses within the last hour. Please try again later.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Your response does not meet the requirements (length or illegal characters).";

                // Genopbyg viewmodel med blog entry, så valideringsfejl kan vises på Reply-siden
                var vm = BuildResponseViewModel(entryId);
                if (vm == null)
                {
                    TempData["Error"] = "The blog entry you are trying to respond to does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                vm.Contents = model.Contents;
                return View(vm);
            }

            var response = new BlogResponse
            {
                Author = userName,
                Contents = model.Contents,
                BlogEntryId = model.BlogEntryId,
                ResponseDate = DateTime.UtcNow
            };

            if (!_blogResponseRepository.CreateBlogResponse(response))
            {
                TempData["Error"] = "An error occurred while saving your response. Please try again later.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Message"] = "Your response has been saved.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string title, string contents)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError(nameof(title), "Title is required.");
            }

            if (string.IsNullOrWhiteSpace(contents))
            {
                ModelState.AddModelError(nameof(contents), "Contents are required.");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var author = User?.Identity?.Name ?? "Admin";
            var blogEntry = _blogEntryRepository.CreateBlogEntry(title, contents, author);

            TempData["Message"] = "Blog entry created successfully.";

            return RedirectToAction(nameof(Index));
        }

        private BlogResponseViewModel? BuildResponseViewModel(int entryId)
        {
            var entry = _blogEntryRepository.GetBlogEntry(entryId);
            if (entry == null)
            {
                return null;
            }

            var entryVm = new BlogEntryViewModel
            {
                Id = entry.Id,
                Title = entry.Title,
                PostedDate = entry.PostedDate,
                Contents = entry.Contents,
                Author = entry.Author
            };

            return new BlogResponseViewModel
            {
                BlogEntryId = entryId,
                BlogEntry = entryVm
            };
        }
    }
}