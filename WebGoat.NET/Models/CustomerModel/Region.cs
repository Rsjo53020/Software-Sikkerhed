using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for region/delstat.
    /// </summary>
    [Owned]
    public class Region
    {
        [Required]
        [StringLength(100, ErrorMessage = "Region must be at most 100 characters.")]
        public string Value { get; private set; } = string.Empty;
        protected Region() { }

        public Region(string value)
        {
            Value = value;
        }
        
        public static implicit operator Region?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new Region(value);

        public override string ToString() => Value;
    }
}