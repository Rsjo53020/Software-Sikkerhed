using Xunit;
using WebGoatCore.Models;
using System;

namespace WebGoat.NET.Tests
{
    public class PhoneNumberUnitTest
    {
        [Theory]
        [InlineData("+45 12345678")]
        [InlineData("123456")]
        [InlineData("(0123) 456789")]
        public void GivenValidPhoneNumber_WhenConstructing_ThenCreatesInstance(string input)
        {
            // When
            var phone = new PhoneNumber(input);

            // Then
            Assert.Equal(input, phone.Value);
        }

        [Theory]
        [InlineData("+45 12345678")]
        [InlineData("123456")]
        [InlineData("(0123) 456789")]
        public void GivenValidPhoneNumber_WhenUsingImplicitConversion_ThenCreatesInstance(string input)
        {
            // When
            PhoneNumber? phone = input;

            // Then
            Assert.NotNull(phone);
            Assert.Equal(input, phone!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenResultIsNull(string? input)
        {
            // When
            PhoneNumber? phone = input;

            // Then
            Assert.Null(phone);
        }

        [Fact]
        public void GivenValidPhoneNumber_WhenCallingToString_ThenReturnsValue()
        {
            // Given
            var value = "+45 12345678";
            var phone = new PhoneNumber(value);

            // When
            var result = phone.ToString();

            // Then
            Assert.Equal(value, result);
        }
    }
}