using System;
using System.Text.RegularExpressions;

/// <summary>
/// Domain Primitive – domain-specifik validering af telefonnummer.
/// Eksempel på at gøre implicit viden eksplicit (Secure by Design kap. 5).
/// </summary>
public sealed class PhoneNumber
{
    private static readonly Regex Pattern = new(@"^[0-9+\-() ]{6,20}$");

    public string Value { get; }

    public PhoneNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Phone number cannot be empty.");

        if (!Pattern.IsMatch(value))
            throw new ArgumentException("Invalid phone number format.");

        Value = value.Trim();
    }
}