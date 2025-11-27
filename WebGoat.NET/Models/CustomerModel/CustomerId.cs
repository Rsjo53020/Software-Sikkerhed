using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain Primitive – sikrer at CustomerId altid er gyldig.
    /// Fjerner shallow models (Secure by Design kap. 5).
    /// </summary>
    [Owned]
    public class CustomerId
    {
        private const string CustomerIdRegex = @"^[A-Z]{5}$";

        [Required]
        [RegularExpression(CustomerIdRegex, ErrorMessage = "CustomerId must be 5 uppercase letters.")]
        public string Value { get; private set; } = string.Empty;

        // For Entity Framework
        protected CustomerId() { }

        public CustomerId(string value)
        {
            if (value is null)
                throw new ArgumentException("CustomerId cannot be null.", nameof(value));

            var trimmed = value.Trim().ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(trimmed))
                throw new ArgumentException("CustomerId cannot be empty.", nameof(value));

            if (!Regex.IsMatch(trimmed, CustomerIdRegex))
                throw new ArgumentException("CustomerId must be 5 uppercase letters.", nameof(value));

            Value = trimmed;
        }

        public static implicit operator CustomerId?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new CustomerId(value);

        public override string ToString() => Value;
    }
}