using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class CustomerIdUnitTest
    {
        [Fact]
        public void GivenValidCustomerId_WhenConstructing_ThenCreatesInstance()
        {
            // Given
            var value = "ABCDE";

            // When
            var id = new CustomerId(value);

            // Then
            Assert.Equal(value, id.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrEmptyCustomerId_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new CustomerId(value!));
        }

        [Theory]
        [InlineData("ABCD")]    // Too short
        [InlineData("ABCDEF")]  // Too long
        [InlineData("AB12C")]   // Contains digits
        public void GivenInvalidFormatCustomerId_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new CustomerId(value));
        }
    }
}