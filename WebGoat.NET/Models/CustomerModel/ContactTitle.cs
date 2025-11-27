using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for kontakt-titel (f.eks. "CEO", "Sales Manager").
    /// Tom/whitespace er ikke tilladt, når den bruges.
    /// </summary>
    [Owned]
    public class ContactTitle
    {
        [Required]
        [StringLength(30, MinimumLength = 1, ErrorMessage = "Contact title must be between 1 and 30 characters.")]
        [RegularExpression(@"^[a-zA-ZÆØÅæøå\s\-]+$", ErrorMessage = "Contact title contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected ContactTitle() { }
        
        public ContactTitle(string value)
        {
            Value = value;
        }
    
        public static implicit operator ContactTitle?(string value)
            => string.IsNullOrWhiteSpace(value) ? null : new ContactTitle(value);

        public override string ToString() => Value;
    }

}