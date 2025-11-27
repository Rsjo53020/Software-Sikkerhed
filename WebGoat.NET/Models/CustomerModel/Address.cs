using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for adresse-linje.
    /// </summary>
    [Owned]
    public class Address
    {
        [Required]
        [StringLength(50, MinimumLength = 3, 
        ErrorMessage = "Address must be between 3 and 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9ÆØÅæøå\s\-]+$", 
        ErrorMessage = "Address contains invalid characters.")]
        public virtual string Value { get; private set; } = string.Empty;

        protected Address() { }

        public Address(string value)
        {
            Value = value;
        }

        public static implicit operator Address?(string value)
        => string.IsNullOrWhiteSpace(value) ? null : new Address(value);

        public override string ToString() => Value;
    }
}