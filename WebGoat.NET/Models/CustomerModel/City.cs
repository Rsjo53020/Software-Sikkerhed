using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for bynavn.
    /// </summary>
    [Owned]
    public class City
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "City must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-ZÆØÅæøå\s\-]+$", ErrorMessage = "City contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected City() { }

        public City(string value)
        {
            Value = value;
        }

        public static implicit operator City?(string value)
        => string.IsNullOrWhiteSpace(value) ? null : new City(value);

        public override string ToString() => Value;
    }
}