using System;
using System.ComponentModel.DataAnnotations;


namespace WebGoatCore.Models
{
    public class BlogResponse
    {
        public int Id { get; set; }

        [Required]
        public int BlogEntryId { get; set; }

        [Required]
        public DateTime ResponseDate { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "Content must be between 5 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9æøåÆØÅ\s\.,()\-\!\?@]+$",
            ErrorMessage = 
            "Content may only contain letters, numbers, spaces, and the characters . , ( ) - ! ? @"
        )]
        public string Contents { get; set; } = string.Empty;

        public virtual BlogEntry? BlogEntry { get; set; }

        
    }
}