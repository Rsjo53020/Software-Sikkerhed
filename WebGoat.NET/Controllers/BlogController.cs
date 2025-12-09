using WebGoatCore.Models;
using WebGoatCore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebGoatCore.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
        public async Task<IActionResult> Index()
        {
            var blogEntryDM = await _blogEntryRepository.GetTopBlogEntriesAsync();

            var viewModels = blogEntryDM.Select(e => new BlogEntryViewModel
            {
                Id = e.Id,
                Title = e.Title.ToString(),
                Contents = e.Contents.ToString(),
                PostedDate = e.PostedDate,
                Author = e.Author.ToString()
            }).ToList();

            return View(viewModels);
        }

        [HttpGet("{entryId:int}")]
        [Authorize]
        public async Task<IActionResult> Reply(int entryId)
        {
            var responseVm = await BuildResponseViewModelAsync(entryId);
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
        public async Task<IActionResult> Reply(int entryId, BlogResponseViewModel model)
        {
            model.BlogEntryId = entryId;
            var userName = User?.Identity?.Name ?? "Anonymous";

            var responsesLastHour = await _blogResponseRepository.CountResponsesByAuthorLastHourAsync(userName);
            
            if (responsesLastHour >= 5)
            {
                TempData["Error"] = "You have already posted 5 responses within the last hour. Please try again later.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Your response does not meet the requirements (length or illegal characters).";

                var vm = await BuildResponseViewModelAsync(entryId);
                if (vm == null)
                {
                    TempData["Error"] = "The blog entry you are trying to respond to does not exist.";
                    return RedirectToAction(nameof(Index));
                }

                vm.Contents = model.Contents;
                return View(vm);
            }

            var blogResponseDM = new BlogResponseDM(
                model.BlogEntryId,
                responseDate: DateTime.Now,
                author: new AuthorName(userName),
                contents: new BlogContent(model.Contents));

            if (!await _blogResponseRepository.CreateBlogResponseAsync(blogResponseDM))
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
        public async Task<IActionResult> Create(string title, string contents)
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

            var blogEntryDM = new BlogEntryDM(
                title: new BlogTitle(title),
                postedDate: DateTime.Now,
                contents: new BlogContent(contents),
                author: new AuthorName(author));

            await _blogEntryRepository.CreateBlogEntryAsync(blogEntryDM);

            TempData["Message"] = "Blog entry created successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<BlogResponseViewModel?> BuildResponseViewModelAsync(int entryId)
        {
            var blogEntryDM = await _blogEntryRepository.GetBlogEntryAsync(entryId);
            if (blogEntryDM == null)
            {
                return null;
            }

            var blogEntryVm = new BlogEntryViewModel
            {
                Id = blogEntryDM.Id,
                Title = blogEntryDM.Title.ToString(),
                PostedDate = blogEntryDM.PostedDate,
                Contents = blogEntryDM.Contents.ToString(),
                Author = blogEntryDM.Author.ToString()
            };

            return new BlogResponseViewModel
            {
                BlogEntryId = entryId,
                BlogEntry = blogEntryVm
            };
        }
    }
}