using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain Primitive – repræsenterer et validt kontakt navn.
    /// Beskytter imod injektion, tomme navne osv.
    /// </summary>
    [Owned]
    public class ContactName
    {
        [Required]
        [StringLength(30, 
        ErrorMessage = "Contact name must be under 30 characters.")]
        [RegularExpression(@"^[a-zA-ZÆØÅæøå\s\-]+$",
        ErrorMessage = "Contact name contains invalid characters."
)]
        public virtual string Value { get; private set; } = string.Empty;

        protected ContactName() { }

        public ContactName(string value)
        {
            Value = value;
        }


        public static implicit operator ContactName?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new ContactName(value);

        public override string ToString() => Value;
    }
}