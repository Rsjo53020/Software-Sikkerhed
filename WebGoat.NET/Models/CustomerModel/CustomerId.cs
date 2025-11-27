using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;



/// <summary>
/// Domain Primitive – sikrer at CustomerId altid er gyldig.
/// Fjerner shallow models (Secure by Design kap. 5).
/// </summary>
[Owned]
public class CustomerId
{
    [Required]
    [RegularExpression(@"^[A-Z]{5}$", ErrorMessage = "CustomerId must be 5 uppercase letters.")]
    public string Value { get; private set; } = string.Empty;
    // For Entity Framework
    protected CustomerId() { }

    public CustomerId(string value)
    {
        Value = value;
    }
    public static implicit operator CustomerId?(string? value)
            => string.IsNullOrWhiteSpace(value) ? null : new CustomerId(value);

    public override string ToString() => Value;
}