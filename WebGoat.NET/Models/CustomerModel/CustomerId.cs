using System;
using System.Text.RegularExpressions;

/// <summary>
/// Domain Primitive – sikrer at CustomerId altid er gyldig.
/// Fjerner shallow models (Secure by Design kap. 5).
/// </summary>
public sealed class CustomerId
{
    private static readonly Regex Pattern = new("^[A-Z]{5}$");

    public string Value { get; }

    public CustomerId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CustomerId cannot be empty.");

        if (!Pattern.IsMatch(value))
            throw new ArgumentException("CustomerId must be 5 uppercase letters.");

        Value = value;
    }
}