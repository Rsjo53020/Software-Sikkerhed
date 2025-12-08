using System;
using System.Collections.Generic;

namespace WebGoatCore.Models
{
    public class BlogEntryDM
    {
        private readonly List<BlogResponseDM> _responses;

        public int Id { get; private set; }
        public BlogTitle Title { get; private set; } = null!;
        public DateTime PostedDate { get; private set; }
        public BlogContent Contents { get; private set; } = null!;
        public AuthorName Author { get; private set; } = null!;

        public IReadOnlyCollection<BlogResponseDM> Responses => _responses.AsReadOnly();

        public BlogEntryDM(
            int Id,
            BlogTitle title,
            DateTime postedDate,
            BlogContent contents,
            AuthorName author)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
            Author = author ?? throw new ArgumentNullException(nameof(author));

            if (postedDate == default)
                throw new ArgumentException("Posted date must be set.", nameof(postedDate));

            PostedDate = postedDate;

            _responses = new List<BlogResponseDM>();
        }

         public BlogEntryDM(
            BlogTitle title,
            DateTime postedDate,
            BlogContent contents,
            AuthorName author)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Contents = contents ?? throw new ArgumentNullException(nameof(contents));
            Author = author ?? throw new ArgumentNullException(nameof(author));

            if (postedDate == default)
                throw new ArgumentException("Posted date must be set.", nameof(postedDate));

            PostedDate = postedDate;

            _responses = new List<BlogResponseDM>();
        }


        public void AddResponse(BlogResponseDM response)
        {
            if (response is null) throw new ArgumentNullException(nameof(response));
            _responses.Add(response);
        }
    }
}