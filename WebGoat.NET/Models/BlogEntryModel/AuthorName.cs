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

        Value = value;
    }

    public override string ToString() => Value;
}