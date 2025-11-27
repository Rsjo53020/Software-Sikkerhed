using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class FaxNumberUnitTest
    {
        [Theory]
        [InlineData("+45 12345678")]
        [InlineData("123456")]
        [InlineData("(0123) 456789")]
        public void GivenValidFaxNumber_WhenConstructing_ThenCreatesInstance(string input)
        {
            // When
            var fax = new FaxNumber(input);

            // Then
            Assert.Equal(input.Trim(), fax.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? input)
        {
            Assert.Throws<ArgumentException>(() => new FaxNumber(input!));
        }

        [Theory]
        [InlineData("123")]                    // For kort
        [InlineData("123456789012345678901")] // For lang (21)
        [InlineData("abc12345")]              // Ugyldige bogstaver
        [InlineData("1234_5678")]             // Ugyldig char _
        public void GivenInvalidFaxNumberFormat_WhenConstructing_ThenThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => new FaxNumber(input));
        }

        [Theory]
        [InlineData("+45 12345678")]
        [InlineData("123456")]
        public void GivenValidFaxNumber_WhenUsingImplicitConversion_ThenCreatesInstance(string input)
        {
            // When
            FaxNumber? fax = input;

            // Then
            Assert.NotNull(fax);
            Assert.Equal(input.Trim(), fax!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenReturnsNull(string? input)
        {
            FaxNumber? fax = input;

            Assert.Null(fax);
        }

        [Fact]
        public void GivenValidFaxNumber_WhenCallingToString_ThenReturnsValue()
        {
            // Given
            var value = "+45 12345678";
            var fax = new FaxNumber(value);

            // When
            var result = fax.ToString();

            // Then
            Assert.Equal(value, result);
        }
    }
}