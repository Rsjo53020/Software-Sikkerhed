using System;

namespace WebGoatCore.Models
{
    public sealed class BlogResponseDM
    {
        public int Id { get; private set; }
        public int BlogEntryId { get; private set; }
        public DateTime ResponseDate { get; private set; }
        public AuthorName Author { get; }
        public BlogContent Contents { get; private set; }

        public BlogResponseDM(
            int blogEntryId,
            DateTime responseDate,
            AuthorName author,
            BlogContent contents
            )
        {
            if (blogEntryId <= 0)
                throw new ArgumentOutOfRangeException(nameof(blogEntryId), "BlogEntryId must be > 0.");

            if (responseDate == default)
                throw new ArgumentException("Response date must be set.", nameof(responseDate));

            Author = author ?? throw new ArgumentNullException(nameof(author));
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));

            BlogEntryId = blogEntryId;
            ResponseDate = responseDate;
        }
    }
}