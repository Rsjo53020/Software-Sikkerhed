using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for faxnummer.
    /// Samme format-regler som for PhoneNumber.
    /// </summary>
    [Owned]
    public class FaxNumber
    {
        private const string FaxRegex = @"^[0-9+\-() ]{6,20}$";

        [Required]
        [RegularExpression(FaxRegex, ErrorMessage = "Fax number contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;

        protected FaxNumber() { }

        public FaxNumber(string value)
        {
            if (value is null)
                throw new ArgumentException("Fax number cannot be null.", nameof(value));

            var trimmed = value.Trim();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("Fax number cannot be empty.", nameof(value));

            if (!Regex.IsMatch(trimmed, FaxRegex))
                throw new ArgumentException("Fax number contains invalid characters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator FaxNumber?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new FaxNumber(value);

        public override string ToString() => Value;
    }
}