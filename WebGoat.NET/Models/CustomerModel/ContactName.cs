using System;

/// <summary>
/// Domain Primitive – repræsenterer et validt kontakt navn.
/// Beskytter imod injektion, tomme navne osv.
/// </summary>
public sealed class ContactName
{
    public string Value { get; }

    public ContactName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Contact name cannot be empty.");

        if (value.Length > 30)
            throw new ArgumentException("Contact name must be under 30 characters.");
        
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-ZÆØÅæøå\s\-]+$"))
            throw new ArgumentException("Contact name contains invalid characters.");

        Value = value.Trim();
    }
}