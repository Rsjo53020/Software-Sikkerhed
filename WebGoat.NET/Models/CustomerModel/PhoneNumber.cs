using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// Domain Primitive – domain-specifik validering af telefonnummer.
/// Eksempel på at gøre implicit viden eksplicit (Secure by Design kap. 5).
/// </summary>
[Owned]
public class PhoneNumber
{

    [Required]
    [RegularExpression(@"^[0-9+\-() ]{6,20}$", ErrorMessage = "Phone number contains invalid characters.")]
    public string Value { get; private set; } = string.Empty;

    protected PhoneNumber() { }
    
    public PhoneNumber(string value)
    {
        Value = value;
    }

    public static implicit operator PhoneNumber?(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : new PhoneNumber(value);

    public override string ToString() => Value;
}