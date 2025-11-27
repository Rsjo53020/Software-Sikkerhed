using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Domain Primitive –> domain-specific validation of phone number.
/// Example of making implicit knowledge explicit (Secure by Design chapter 5).
/// </summary>
[Owned]
public class PhoneNumber
{
    private const string PhoneRegex = @"^[0-9+\-() ]{6,20}$";

    [Required]
    [RegularExpression(PhoneRegex, ErrorMessage = "Phone number contains invalid characters.")]
    public string Value { get; private set; } = string.Empty;

    protected PhoneNumber() { }
    
    public PhoneNumber(string value)
    {
        if (value is null)
            throw new ArgumentException("Phone number cannot be null.", nameof(value));

        var trimmed = value.Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
            throw new ArgumentException("Phone number cannot be empty.", nameof(value));

        if (!Regex.IsMatch(trimmed, PhoneRegex))
            throw new ArgumentException("Phone number contains invalid characters.", nameof(value));

        Value = trimmed;
    }

    public static implicit operator PhoneNumber?(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : new PhoneNumber(value);

    public override string ToString() => Value;
}