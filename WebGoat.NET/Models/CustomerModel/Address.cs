using System;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for adresse-linje.
    /// </summary>
    public sealed class Address
    {
        public string Value { get; }

        public Address(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Address cannot be empty.");

            value = value.Trim();

            // length check bewetween 3 and 50 characters
            if (value.Length < 3 || value.Length > 50)
                throw new ArgumentException("Address must be between 3 and 50 characters.");

            // regex for no special characters other than space,and hyphen
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9ÆØÅæøå\s\-]+$"))
                throw new ArgumentException("Address contains invalid characters.");
                
            Value = value;

        }
    }
}