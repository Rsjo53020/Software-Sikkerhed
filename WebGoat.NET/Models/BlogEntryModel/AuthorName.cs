using System;
using System.ComponentModel.DataAnnotations;

public sealed class AuthorName
{
    public const int MaxLength = 100;

    [Required]
    [StringLength(100)]
    public string Value { get; }

    public AuthorName(string value)
    {
        value = value.Trim();

        if (value.Length > MaxLength)
            throw new ArgumentException(
                $"Author name cannot exceed {MaxLength} characters.",
                nameof(value));

        if (string.IsNullOrEmpty(value))
            throw new ArgumentException(
                    $"Author name cannot be' Empty.",
                    nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}