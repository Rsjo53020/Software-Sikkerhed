using System;
using System.Text.RegularExpressions;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for faxnummer.
    /// Samme format-regler som for PhoneNumber.
    /// </summary>
    public sealed class FaxNumber
    {
        private static readonly Regex Pattern = new(@"^[0-9+\-() ]{6,20}$");

        public string Value { get; }

        public FaxNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Fax number cannot be empty.");

            value = value.Trim();

            if (!Pattern.IsMatch(value))
                throw new ArgumentException("Invalid fax number format.");

            Value = value;
        }
    }
}