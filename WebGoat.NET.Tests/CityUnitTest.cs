using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class CityUnitTests
    {
        [Theory]
        [InlineData("  Aarhus  ", "Aarhus")]
        [InlineData("København", "København")]
        [InlineData("  Aalborg Øst  ", "Aalborg Øst")]
        public void GivenValidCity_WhenConstructing_ThenReturnsTrimmedInstance(string input, string expected)
        {
            // When
            var city = new City(input);

            // Then
            Assert.Equal(expected, city.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new City(value!));
        }

        [Theory]
        [InlineData("Aa")]                            // For kort (2)
        public void GivenCityWithInvalidLength_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new City(value));
        }

        [Theory]
        [InlineData("Aarhus1")]   // tal ikke tilladt
        [InlineData("Århus!")]    // ! ikke tilladt
        [InlineData("By#Navn")]   // # ikke tilladt
        public void GivenCityWithInvalidCharacters_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new City(value));
        }

        [Fact]
        public void GivenValidCity_WhenUsingImplicitConversion_ThenCreatesTrimmedInstance()
        {
            // Given
            string input = "  Randers  ";

            // When
            City? city = input;

            // Then
            Assert.NotNull(city);
            Assert.Equal("Randers", city!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenReturnsNull(string? value)
        {
            // When
            City? city = value;

            // Then
            Assert.Null(city);
        }

        [Fact]
        public void GivenValidCity_WhenCallingToString_ThenReturnsValue()
        {
            // Given
            var city = new City("Odense");

            // When
            var result = city.ToString();

            // Then
            Assert.Equal("Odense", result);
        }
    }
}