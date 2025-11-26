using System;
using System.Text.RegularExpressions;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for postnummer.
    /// Simpel regel: 3-12 tegn, kun tal/bogstaver/mellemrum/bindestreg.
    /// </summary>
    public sealed class PostalCode
    {
        private static readonly Regex Pattern = new(@"^[0-9A-Za-z\- ]{3,12}$");

        public string Value { get; }

        public PostalCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Postal code cannot be empty.");

            value = value.Trim();

            if (!Pattern.IsMatch(value))
                throw new ArgumentException("Invalid postal code format.");

            Value = value;
        }
    }
}