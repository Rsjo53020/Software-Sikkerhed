using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for adresse-linje.
    /// </summary>
    [Owned]
    public class Address
    {
        private const int MinLength = 3;
        private const int MaxLength = 50;
        private const string AddressRegex = @"^[a-zA-Z0-9ÆØÅæøå\s\-]+$";

        [Required]
        [StringLength(MaxLength, MinimumLength = MinLength,
            ErrorMessage = "Address must be between 3 and 50 characters.")]
        [RegularExpression(AddressRegex,
            ErrorMessage = "Address contains invalid characters.")]
        public virtual string Value { get; private set; } = string.Empty;

        protected Address() { }

        public Address(string value)
        {
            if (value is null)
                throw new ArgumentException("Address cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Address cannot be empty.", nameof(value));

            if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
                throw new ArgumentException(
                    $"Address must be between {MinLength} and {MaxLength} characters.",
                    nameof(value));

            if (!Regex.IsMatch(trimmed, AddressRegex))
                throw new ArgumentException("Address contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator Address?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new Address(value);

        public override string ToString() => Value;
    }
}