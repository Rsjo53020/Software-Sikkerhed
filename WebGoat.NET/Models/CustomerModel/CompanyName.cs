using System;

namespace WebGoatCore.Models
{
/// <summary>
/// Domain Primitive – definerer præcis hvad en CompanyName må være.
/// Der må ikke eksistere ugyldige navne i systemet.
/// </summary>
public sealed class CompanyName
{
    public string Value { get; }

    public CompanyName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Company name cannot be empty.");

        if (value.Length > 100)
            throw new ArgumentException("Company name must be under 100 characters.");

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9ÆØÅæøå\s\-]+$"))
            throw new ArgumentException("Company name contains invalid characters.");

        Value = value.Trim();
    }
}
}