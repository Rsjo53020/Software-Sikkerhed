using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class CountryUnitTests
    {
        [Theory]
        [InlineData("dk", "DK")]
        [InlineData(" se ", "SE")]
        [InlineData("US", "US")]
        public void GivenValidCountryCode_WhenConstructing_ThenReturnsUppercaseTrimmedCode(string input, string expected)
        {
            // When
            var country = new Country(input);

            // Then
            Assert.Equal(expected, country.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => new Country(value!));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenReturnsNull(string? value)
        {
            // When
            Country? result = value;

            // Then
            Assert.Null(result);
        }

        [Fact]
        public void GivenValidCountryCode_WhenCallingToString_ThenReturnsValue()
        {
            // Given
            var country = new Country("NL");

            // When
            var result = country.ToString();

            // Then
            Assert.Equal("NL", result);
        }
    }
}