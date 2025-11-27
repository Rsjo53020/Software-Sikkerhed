using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain Primitive – repræsenterer et validt kontakt navn.
    /// Beskytter imod injektion, tomme navne osv.
    /// </summary>
    [Owned]
    public class ContactName
    {
        private const int MaxLength = 30;
        private const string NameRegex = @"^[a-zA-ZÆØÅæøå\s\-]+$";

        [Required]
        [StringLength(MaxLength,
            ErrorMessage = "Contact name must be under 30 characters.")]
        [RegularExpression(NameRegex,
            ErrorMessage = "Contact name contains invalid characters.")]
        public virtual string Value { get; private set; } = string.Empty;

        protected ContactName() { }

        public ContactName(string value)
        {
            if (value is null)
                throw new ArgumentException("Contact name cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Contact name cannot be empty.", nameof(value));

            if (trimmed.Length > MaxLength)
                throw new ArgumentException($"Contact name must be under {MaxLength} characters.", nameof(value));

            if (!Regex.IsMatch(trimmed, NameRegex))
                throw new ArgumentException("Contact name contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator ContactName?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new ContactName(value);

        public override string ToString() => Value;
    }
}