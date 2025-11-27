using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for landekode.
    /// Her bruger vi ISO 3166-1 alpha-2 (f.eks. "DK", "SE", "US").
    /// </summary>
    [Owned]
    public class Country
    {
        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country must be a 2-letter ISO code.")]
        [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "Country contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected Country() { }
        
        public Country(string value)
        {
            Value = value;
        }

        public static implicit operator Country?(string value)
            => string.IsNullOrWhiteSpace(value) ? null : new Country(value);

        public override string ToString() => Value;
    }
}
