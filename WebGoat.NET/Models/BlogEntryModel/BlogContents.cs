using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public sealed class BlogContent
{
    public const int MinLength = 5;
    public const int MaxLength = 2000;

    public const string AllowedCharactersPattern = @"^[^<>=]+$";

    [Required]
    [StringLength(
        MaxLength,
        MinimumLength = MinLength,
        ErrorMessage = $"Content must be between 5 and 2000 characters."
    )]
    public string Value { get; }

    public BlogContent(string value)
    {
        if (value is null)
            throw new ArgumentException("Content cannot be null.", nameof(value));

        var trimmed = value.Trim();

        if (trimmed.Length < MinLength || trimmed.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Content must be between {MinLength} and {MaxLength} characters.",
                nameof(value));
        }

        Value = trimmed;
    }

    public override string ToString() => Value;
}