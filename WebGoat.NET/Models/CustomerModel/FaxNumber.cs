using System;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        [RegularExpression(@"^[0-9+\-() ]{6,20}$", ErrorMessage = "Fax number contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;
        protected FaxNumber() { }
        
        public FaxNumber(string value)
        {
            Value = value;
        }

        public static implicit operator FaxNumber?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new FaxNumber(value);

        public override string ToString() => Value;

    }
}