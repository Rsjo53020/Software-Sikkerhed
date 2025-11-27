using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class ContactNameUnitTest
    {
        [Theory]
        [InlineData("  Hans Hansen  ", "Hans Hansen")]
        [InlineData("Åse Øster", "Åse Øster")]
        [InlineData("Lars-Peter", "Lars-Peter")]
        public void GivenValidName_WhenConstructing_ThenReturnsTrimmedInstance(string input, string expected)
        {
            // When
            var name = new ContactName(input);

            // Then
            Assert.Equal(expected, name.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new ContactName(value!));
        }

        [Fact]
        public void GivenNameExceedingMaxLength_WhenConstructing_ThenThrowsArgumentException()
        {
            // Given
            var longName = new string('A', 31);

            // When + Then
            Assert.Throws<ArgumentException>(() => new ContactName(longName));
        }

        [Theory]
        [InlineData("Hans1")]   // tal ikke tilladt
        [InlineData("Hans!")]   // ! ikke tilladt
        [InlineData("Hans_")]   // _ ikke tilladt
        public void GivenNameWithInvalidCharacters_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new ContactName(value));
        }

        [Fact]
        public void GivenValidName_WhenUsingImplicitConversion_ThenCreatesTrimmedInstance()
        {
            // Given
            string input = "  Hans Hansen  ";

            // When
            ContactName? name = input;

            // Then
            Assert.NotNull(name);
            Assert.Equal("Hans Hansen", name!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenReturnsNull(string? value)
        {
            // When
            ContactName? name = value;

            // Then
            Assert.Null(name);
        }

        [Fact]
        public void GivenValidName_WhenCallingToString_ThenReturnsValue()
        {
            // Given
            var name = new ContactName("Hans Hansen");

            // When
            var result = name.ToString();

            // Then
            Assert.Equal("Hans Hansen", result);
        }
    }
}