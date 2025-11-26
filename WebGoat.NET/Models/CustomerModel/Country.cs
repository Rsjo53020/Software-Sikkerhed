using System;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for landekode.
    /// Her bruger vi ISO 3166-1 alpha-2 (f.eks. "DK", "SE", "US").
    /// </summary>
    public sealed class Country
    {
        public string Value { get; }

        public Country(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Country cannot be empty.");

            value = value.Trim().ToUpperInvariant();

            if (value.Length != 2)
                throw new ArgumentException("Country must be a 2-letter ISO code.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[A-Z]{2}$"))
                throw new ArgumentException("Country contains invalid characters.");

            Value = value;
        }
    }
}
