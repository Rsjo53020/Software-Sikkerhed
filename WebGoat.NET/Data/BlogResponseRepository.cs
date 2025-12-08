using WebGoatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;


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
        public async Task<int> CountResponsesByAuthorLastHourAsync(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return 0;

            var normalizedAuthor = author.Trim();
            var since = DateTime.Now.AddHours(-1);

            return await _context.BlogResponses
                .AsNoTracking()
                .Where(r => r.Author == normalizedAuthor && r.ResponseDate >= since)
                .CountAsync();
        }
        public async Task<bool> CreateBlogResponseAsync(BlogResponseDM response)
        {
            try
            {
                //Map from BlogResponseDM to BlogResponse
                var blogResponse = new BlogResponse(
                    blogEntryId: response.BlogEntryId,
                    responseDate: response.ResponseDate,
                    author: response.Author.ToString(),
                    contents: response.Contents.ToString()
                );

                await _context.BlogResponses.AddAsync(blogResponse);
                await _context.SaveChangesAsync();
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