using WebGoatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;


namespace WebGoatCore.Data
{
    public class BlogResponseRepository
    {
        private readonly ILogger<BlogResponseRepository> _logger;
        private readonly NorthwindContext _context;
        
        public BlogResponseRepository(NorthwindContext context, ILogger<BlogResponseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public int CountResponsesByAuthorLastHour(string author)
        {
            var since = DateTime.Now.AddHours(-1);
            return _context.BlogResponses
                .Where(r => r.Author == author && r.ResponseDate >= since)
                .Count();
        }
        public bool CreateBlogResponse(BlogResponse response)
        {
            try
            {
                _context.BlogResponses.Add(response);
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving BlogResponse {@BlogResponse}", response);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving BlogResponse {@BlogResponse}", response);
                return false;
            }
        }
    }
}