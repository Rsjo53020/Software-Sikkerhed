using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class ContactNameUnitTests
    {
        [Fact]
        public void GivenValidContactName_WhenConstructing_ThenReturnsTrimmedInstance()
        {
            // Given
            var input = "  John Doe  ";

            // When
            var result = new ContactName(input);

            // Then
            Assert.Equal("John Doe", result.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrEmptyContactName_WhenConstructing_ThenThrowsArgumentException(string? input)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new ContactName(input!));
        }

        [Fact]
        public void GivenTooLongContactName_WhenConstructing_ThenThrowsArgumentException()
        {
            // Given
            var tooLong = new string('X', 81);

            // When + Then
            Assert.Throws<ArgumentException>(() => new ContactName(tooLong));
        }
    }
}