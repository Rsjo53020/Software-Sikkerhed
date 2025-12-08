using System;

namespace WebGoatCore.Models
{
    public class BlogResponse
    {
        public int Id { get; private set; }
        public int BlogEntryId { get; private set; }
        public DateTime ResponseDate { get; private set; }
        public string Author { get; private set; } = string.Empty;
        public string Contents { get; private set; } = string.Empty;
        public virtual BlogEntry? BlogEntry { get; private set; }

        public BlogResponse(int blogEntryId, DateTime responseDate, string author, string contents)
        {
            BlogEntryId = blogEntryId;
            ResponseDate = responseDate;
            Author = author;
            Contents = contents;
        }
    }
}