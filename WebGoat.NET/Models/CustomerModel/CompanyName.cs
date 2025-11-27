using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
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
        private const int MaxLength = 100;
        private const string NameRegex = @"^[a-zA-Z0-9ÆØÅæøå\s\-]+$";

        [Required]
        [StringLength(MaxLength, MinimumLength = 1, ErrorMessage = "Company name must be between 1 and 100 characters.")]
        [RegularExpression(NameRegex, ErrorMessage = "Company name contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected CompanyName() { }

        public CompanyName(string value)
        {
            if (value is null)
                throw new ArgumentException("Company name cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Company name cannot be empty.", nameof(value));

            if (trimmed.Length > MaxLength)
                throw new ArgumentException($"Company name cannot be longer than {MaxLength} characters.", nameof(value));

            if (!Regex.IsMatch(trimmed, NameRegex))
                throw new ArgumentException("Company name contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator CompanyName?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new CompanyName(value);

        public override string ToString() => Value;
    }
}