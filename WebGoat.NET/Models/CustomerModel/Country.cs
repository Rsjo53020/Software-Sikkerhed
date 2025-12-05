using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Country must be a 2-letter ISO code.")]
        public string Value { get; private set; } = string.Empty;

        protected Country() { }

        public Country(string value)
        {
            if (value is null)
                throw new ArgumentException("Country cannot be null.", nameof(value));

            var trimmed = value.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Country cannot be empty.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator Country?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new Country(value);

        public override string ToString() => Value;
    }
}