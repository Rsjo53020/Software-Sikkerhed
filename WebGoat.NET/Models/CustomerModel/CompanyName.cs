using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain Primitive – definerer præcis hvad en CompanyName må være.
    /// Der må ikke eksistere ugyldige navne i systemet.
    /// </summary>
    [Owned]
    public class CompanyName
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Company name must be between 1 and 100 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9ÆØÅæøå\s\-]+$", ErrorMessage = "Company name contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected CompanyName() { }

        public CompanyName(string value)
        {
            Value = value;
        }

        public static implicit operator CompanyName?(string value)
            => string.IsNullOrWhiteSpace(value) ? null : new CompanyName(value);

        public override string ToString() => Value;
    }
}