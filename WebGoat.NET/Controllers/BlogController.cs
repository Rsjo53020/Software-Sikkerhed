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

        public BlogController(BlogEntryRepository blogEntryRepository, BlogResponseRepository blogResponseRepository, NorthwindContext context)
        {
            _blogEntryRepository = blogEntryRepository;
            _blogResponseRepository = blogResponseRepository;
        }

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

        [HttpGet("{entryId}")]
        public IActionResult Reply(int entryId)
        {
            var entry = _blogEntryRepository.GetBlogEntry(entryId);

            if (entry == null)
            {
                TempData["Error"] = "The blog entry you are trying to respond to does not exist.";
                return RedirectToAction("Index");
            }
            var entryVm = new BlogEntryViewModel
            {
                Id = entry.Id,
                Title = entry.Title,
                PostedDate = entry.PostedDate,
                Contents = entry.Contents,
                Author = entry.Author
            };

            var responseVm = new BlogResponseViewModel
            {
                BlogEntryId = entryId,
                BlogEntry = entryVm
            };

            return View(responseVm);
        }

        [HttpPost("{entryId}")]
        public IActionResult Reply(BlogResponseViewModel model)
        {

            var userName = User?.Identity?.Name ?? "Anonymous";

            // RATE LIMIT: max 5 responses per hour per user
            var responsesLastHour = _blogResponseRepository.CountResponsesByAuthorLastHour(userName);
            if (responsesLastHour >= 5)
            {
                TempData["Error"] = "You have already posted 5 responses within the last hour. Please try again later.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Your response does not meet the requirements (length or illegal characters).";
                return RedirectToAction("Index");
            }

            var response = new BlogResponse
            {
                Author = userName,
                Contents = model.Contents,
                BlogEntryId = model.BlogEntryId,
                ResponseDate = DateTime.Now
            };

            if (!_blogResponseRepository.CreateBlogResponse(response))
            {
                TempData["Error"] = "An error occurred while saving your response. Please try again later.";
                return RedirectToAction("Index");
            }

            TempData["Message"] = "Your response has been saved.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(string title, string contents)
        {
            var blogEntry = _blogEntryRepository.CreateBlogEntry(title, contents, User!.Identity!.Name!);
            return View(blogEntry);
        }
    }
}