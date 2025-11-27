using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for postnummer.
    /// Simple regel: 3-12 tegn, kun tal/bogstaver/mellemrum/bindestreg.
    /// </summary>
    [Owned]
    public class PostalCode
    {
        private const string PostalRegex = @"^[0-9A-Za-z\- ]{3,12}$";

        [Required]
        [RegularExpression(PostalRegex,
            ErrorMessage = "Postal code contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected PostalCode() { }

        public PostalCode(string value)
        {
            if (value is null)
                throw new ArgumentException("Postal code cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Postal code cannot be empty.", nameof(value));

            if (!Regex.IsMatch(trimmed, PostalRegex))
                throw new ArgumentException("Postal code contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator PostalCode?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new PostalCode(value);

        public override string ToString() => Value;
    }
}