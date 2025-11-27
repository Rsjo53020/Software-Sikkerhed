using System;
using System.Collections.Generic;

namespace WebGoatCore.ViewModels
{
    public class BlogEntryViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; }
        public string Contents { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;

        public virtual IList<BlogResponseViewModel>? Responses { get; set; }
    }
}