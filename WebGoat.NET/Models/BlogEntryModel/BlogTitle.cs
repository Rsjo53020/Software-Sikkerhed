using System;

public sealed class BlogTitle
{
    public const int MaxLength = 200;

    public string Value { get; }

    public BlogTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Title is required.", nameof(value));

        value = value.Trim();

        if (value.Length > MaxLength)
            throw new ArgumentException($"Title must be at most {MaxLength} characters.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value;
}