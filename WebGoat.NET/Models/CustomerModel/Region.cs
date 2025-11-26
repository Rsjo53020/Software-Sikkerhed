using System;

namespace WebGoatCore.Models
{
    /// <summary>
    /// Domain primitive for region/delstat.
    /// </summary>
    public sealed class Region
    {
        public string Value { get; }

        public Region(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Region cannot be empty.");

            value = value.Trim();

            if (value.Length > 100)
                throw new ArgumentException("Region must be at most 100 characters.");

            Value = value;
        }
    }
}