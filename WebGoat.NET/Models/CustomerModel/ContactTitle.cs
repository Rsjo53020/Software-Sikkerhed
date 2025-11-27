using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// In english Domain primitive for contact title (e.g. "CEO", "Sales Manager").
    /// Empty/whitespace is not allowed when used.
    /// </summary>
    [Owned]
    public class ContactTitle
    {
        private const int MaxLength = 30;
        private const int MinLength = 1;
        private const string TitleRegex = @"^[a-zA-ZÆØÅæøå\s\-]+$";

        [Required]
        [StringLength(MaxLength, MinimumLength = MinLength,
            ErrorMessage = "Contact title must be between 1 and 30 characters.")]
        [RegularExpression(TitleRegex,
            ErrorMessage = "Contact title contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected ContactTitle() { }

        public ContactTitle(string value)
        {
            if (value is null)
                throw new ArgumentException("Contact title cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Contact title cannot be empty.", nameof(value));

            if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
                throw new ArgumentException($"Contact title must be between {MinLength} and {MaxLength} characters.", nameof(value));

            if (!Regex.IsMatch(trimmed, TitleRegex))
                throw new ArgumentException("Contact title contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator ContactTitle?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new ContactTitle(value);

        public override string ToString() => Value;
    }
}