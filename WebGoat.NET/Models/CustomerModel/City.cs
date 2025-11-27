using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for bynavn.
    /// </summary>
    [Owned]
    public class City
    {
        private const int MaxLength = 50;
        private const int MinLength = 3;
        private const string CityRegex = @"^[a-zA-ZÆØÅæøå\s\-]+$";

        [Required]
        [StringLength(MaxLength, MinimumLength = MinLength, ErrorMessage = "City must be between 3 and 50 characters.")]
        [RegularExpression(CityRegex, ErrorMessage = "City contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected City() { }

        public City(string value)
        {
            if (value is null)
                throw new ArgumentException("City cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("City cannot be empty.", nameof(value));

            if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
                throw new ArgumentException($"City must be between {MinLength} and {MaxLength} characters.", nameof(value));

            if (!Regex.IsMatch(trimmed, CityRegex))
                throw new ArgumentException("City contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator City?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new City(value);

        public override string ToString() => Value;
    }
}