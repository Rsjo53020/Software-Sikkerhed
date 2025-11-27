using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class AddressUnitTests
    {
        [Theory]
        [InlineData("  Nørregade 12  ", "Nørregade 12")]
        [InlineData("Hovedvejen 5", "Hovedvejen 5")]
        [InlineData("Aabygade-vej 3", "Aabygade-vej 3")]
        public void GivenValidAddress_WhenConstructing_ThenReturnsTrimmedInstance(string input, string expected)
        {
            // When
            var address = new Address(input);

            // Then
            Assert.Equal(expected, address.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new Address(value!));
        }

        [Theory]
        [InlineData("Aa")]                                // for kort
        public void GivenAddressWithInvalidLength_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new Address(value));
        }

        [Theory]
        [InlineData("Nørregade 12, 1. th")] // komma + punktum
        [InlineData("Vej #12")]             // #
        [InlineData("Vej_12")]              // _
        public void GivenAddressWithInvalidCharacters_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new Address(value));
        }

        [Fact]
        public void GivenValidAddress_WhenUsingImplicitConversion_ThenCreatesTrimmedInstance()
        {
            // Given
            string input = "  Vesterbrogade 100  ";

            // When
            Address? address = input;

            // Then
            Assert.NotNull(address);
            Assert.Equal("Vesterbrogade 100", address!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenUsingImplicitConversion_ThenReturnsNull(string? value)
        {
            // When
            Address? address = value;

            // Then
            Assert.Null(address);
        }

        [Fact]
        public void GivenValidAddress_WhenCallingToString_ThenReturnsValue()
        {
            // Given
            var address = new Address("Nørregade 12");

            // When
            var result = address.ToString();

            // Then
            Assert.Equal("Nørregade 12", result);
        }
    }
}