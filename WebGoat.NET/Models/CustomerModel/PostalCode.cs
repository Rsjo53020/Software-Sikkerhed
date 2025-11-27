using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for postnummer.
    /// Simpel regel: 3-12 tegn, kun tal/bogstaver/mellemrum/bindestreg.
    /// </summary>
    [Owned]
    public class PostalCode
    {
        [Required]
        [RegularExpression(@"^[0-9A-Za-z\- ]{3,12}$", ErrorMessage = "Postal code contains invalid characters.")]
        public string Value { get; private set; } = string.Empty;
        protected PostalCode() { }

        public PostalCode(string value)
        {
            Value = value;
        }
        
        public static implicit operator PostalCode?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new PostalCode(value);

        public override string ToString() => Value;
    }
}