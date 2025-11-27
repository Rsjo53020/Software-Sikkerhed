using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class ContactTitleUnitTest
    {
        [Theory]
        [InlineData("  Sales Manager  ", "Sales Manager")]
        [InlineData("Direktør", "Direktør")]
        [InlineData("Key-Account Manager", "Key-Account Manager")]
        public void GivenValidTitle_WhenConstructing_ThenReturnsTrimmedInstance(string input, string expected)
        {
            // When
            var title = new ContactTitle(input);

            // Then
            Assert.Equal(expected, title.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => new ContactTitle(value!));
        }

        [Theory]
        [InlineData("CEO1")]  // tal ikke tilladt
        [InlineData("Manager!")] // ! ikke tilladt
        [InlineData("Sales_Manager")] // _ ikke tilladt
        public void GivenTitleWithInvalidCharacters_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => new ContactTitle(value));
        }

        [Fact]
        public void GivenValidTitle_WhenUsingImplicitConversion_ThenCreatesTrimmedInstance()
        {
            // Given
            string input = "  CEO  ";

            // When
            ContactTitle? title = input;

            // Then
            Assert.NotNull(title);
            Assert.Equal("CEO", title!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenReturnsNull(string? value)
        {
            ContactTitle? title = value;
            Assert.Null(title);
        }

        [Fact]
        public void GivenValidTitle_WhenCallingToString_ThenReturnsValue()
        {
            var title = new ContactTitle("CEO");
            Assert.Equal("CEO", title.ToString());
        }
    }
}