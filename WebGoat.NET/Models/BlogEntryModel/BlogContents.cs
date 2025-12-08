using System;
using System.ComponentModel.DataAnnotations;

public sealed class BlogContent
{
    [Required]
        [StringLength(500, MinimumLength = 5,
            ErrorMessage = "Content must be between 5 and 500 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9æøåÆØÅ\s\.,()\-\!\?@]+$",
            ErrorMessage = 
            "Content may only contain letters, numbers, spaces, and the characters . , ( ) - ! ? @"
        )]
    public string Value { get; }

    public BlogContent(string value)
    {
        Value = value;
    }

    public override string ToString() => Value;
}