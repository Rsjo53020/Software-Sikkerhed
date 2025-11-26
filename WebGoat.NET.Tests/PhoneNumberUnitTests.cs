using Xunit;
using WebGoatCore.Models;
using System;

namespace WebGoat.NET.Tests
{
    public class PhoneNumberTests
    {
        [Theory]
        [InlineData("+45 12345678")]
        [InlineData("123456")]
        [InlineData("(0123) 456789")]
        public void GivenValidPhoneNumber_WhenConstructing_ThenCreatesInstance(string input)
        {
            // Given
            var value = input;

            // When
            var phone = new PhoneNumber(value);

            // Then
            Assert.Equal(value.Trim(), phone.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrEmptyPhoneNumber_WhenConstructing_ThenThrowsArgumentException(string? input)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new PhoneNumber(input!));
        }

        [Theory]
        [InlineData("123")]                  // For kort
        [InlineData("123456789012345678901")] // For lang (21)
        [InlineData("abc12345")]            // Ugyldige bogstaver
        [InlineData("1234_5678")]           // Ugyldig char _
        public void GivenInvalidPhoneNumberFormat_WhenConstructing_ThenThrowsArgumentException(string input)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new PhoneNumber(input));
        }
    }
}