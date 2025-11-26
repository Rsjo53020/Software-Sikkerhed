using System;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for bynavn.
    /// </summary>
    public sealed class City
    {
        public string Value { get; }

        public City(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("City cannot be empty.");

            value = value.Trim();

            if (value.Length > 50)
                throw new ArgumentException("City must be at most 50 characters.");

            // regex for no special characters other than space,and hyphen
            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-ZÆØÅæøå\s\-]+$"))
                throw new ArgumentException("City contains invalid characters.");

            Value = value;
        }
    }
}