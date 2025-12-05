using System.ComponentModel.DataAnnotations;

namespace WebGoatCore.ViewModels
{
    public class BlogResponseViewModel
    {
        [Required]
        public int BlogEntryId { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "Content must be between 5 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9æøåÆØÅ\s\.,()\-\!\?@]+$",
            ErrorMessage = 
            "Content may only contain letters, numbers, spaces, and the characters . , ( ) - ! ? @"
        )]
        public string Contents { get; set; } = string.Empty;

        public BlogEntryViewModel? BlogEntry { get; set; }
    }
}