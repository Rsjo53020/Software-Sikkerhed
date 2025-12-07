using WebGoatCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebGoatCore.Data
{
    public class BlogEntryRepository
    {
        private readonly NorthwindContext _context;

        public BlogEntryRepository(NorthwindContext context)
        {
            _context = context;
        }

        public async Task<BlogEntry> CreateBlogEntryAsync(BlogEntry entry)
        {
            await _context.BlogEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<BlogEntry?> GetBlogEntryAsync(int blogEntryId)
        {
            return await _context.BlogEntries
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == blogEntryId);
        }

        public async Task<IReadOnlyList<BlogEntry>> GetTopBlogEntriesAsync(int numberOfEntries = 4, int startPosition = 0)
        {
            if (numberOfEntries <= 0)
                return Array.Empty<BlogEntry>();

            if (startPosition < 0)
                startPosition = 0;

            return await _context.BlogEntries
                .AsNoTracking()
                .OrderByDescending(b => b.PostedDate)
                .Skip(startPosition)
                .Take(numberOfEntries)
                .ToListAsync();
        }
    }
}