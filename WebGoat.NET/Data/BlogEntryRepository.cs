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

        public async Task<BlogEntry> CreateBlogEntryAsync(BlogEntryDM entryDM)
        {
            var entry = new BlogEntry
            {
                Id = entryDM.Id,
                Title = entryDM.Title.ToString(),
                Author = entryDM.Author.ToString(),
                Contents = entryDM.Contents.ToString(),
                PostedDate = entryDM.PostedDate
            };

            await _context.BlogEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
            return entry;
        }

        public async Task<BlogEntryDM> GetBlogEntryAsync(int blogEntryId)
        {
            var response = await _context.BlogEntries
                .AsNoTracking()
                .SingleOrDefaultAsync(b => b.Id == blogEntryId);

            var blogEntryDM = new BlogEntryDM(
                title: new BlogTitle(response.Title),
                postedDate: response.PostedDate,
                contents: new BlogContent(response.Contents),
                author: new AuthorName(response.Author)
            );

            return blogEntryDM;
        }

        public async Task<IReadOnlyList<BlogEntryDM>> GetTopBlogEntriesAsync(int numberOfEntries = 4, int startPosition = 0)
        {
            if (numberOfEntries <= 0)
                return Array.Empty<BlogEntryDM>();

            if (startPosition < 0)
                startPosition = 0;

            var response = await _context.BlogEntries
                .AsNoTracking()
                .OrderByDescending(b => b.PostedDate)
                .Skip(startPosition)
                .Take(numberOfEntries)
                .ToListAsync();

            //map from BlogEntry to BlogEntryDM
            return response
                .Select(b => new BlogEntryDM(
                    Id: b.Id,
                    title: new BlogTitle(b.Title),
                    postedDate: b.PostedDate,
                    author: new AuthorName(b.Author),
                    contents: new BlogContent(b.Contents)
                )).ToList();
        }
    }
}