using System;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for kontakt-titel (f.eks. "CEO", "Sales Manager").
    /// Tom/whitespace er ikke tilladt, når den bruges.
    /// </summary>
    public sealed class ContactTitle
    {
        public string Value { get; }

        public ContactTitle(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Contact title cannot be empty.");

            value = value.Trim();

            if (value.Length > 30)
                throw new ArgumentException("Contact title must be at most 30 characters.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-ZÆØÅæøå\s\-]+$"))
                throw new ArgumentException("Contact title contains invalid characters.");

            Value = value;
        }
    }
}