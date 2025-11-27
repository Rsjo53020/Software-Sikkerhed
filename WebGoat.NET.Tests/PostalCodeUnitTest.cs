using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Test
{
    public class PostalCodeUnitTests
    {
        [Theory]
        [InlineData("  8000  ", "8000")]
        [InlineData("DK-2100", "DK-2100")]
        [InlineData("SW 1A", "SW 1A")]
        public void GivenValidPostalCode_WhenConstructing_ThenReturnsTrimmedValue(string input, string expected)
        {
            var code = new PostalCode(input);
            Assert.Equal(expected, code.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenConstructing_ThenThrowsArgumentException(string? value)
        {
            Assert.Throws<ArgumentException>(() => new PostalCode(value!));
        }

        [Theory]
        [InlineData("A")]                   // for kort
        [InlineData("ABCDEFGHIJKLMNOP")]    // for lang (13)
        [InlineData("12_345")]              // underscore
        [InlineData("12.345")]              // punktum
        [InlineData("12/345")]              // slash
        public void GivenInvalidPostalCodeFormat_WhenConstructing_ThenThrowsArgumentException(string value)
        {
            Assert.Throws<ArgumentException>(() => new PostalCode(value));
        }

        [Theory]
        [InlineData(" 8000 ", "8000")]
        [InlineData("DK-1000", "DK-1000")]
        public void GivenValidPostalCode_WhenImplicitCast_ThenCreatesInstance(string input, string expected)
        {
            PostalCode? code = input;
            Assert.NotNull(code);
            Assert.Equal(expected, code!.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void GivenNullOrWhitespace_WhenImplicitCast_ThenReturnsNull(string? value)
        {
            PostalCode? code = value;
            Assert.Null(code);
        }

        [Fact]
        public void GivenValidPostalCode_WhenCallingToString_ThenReturnsValue()
        {
            var code = new PostalCode("1234");
            Assert.Equal("1234", code.ToString());
        }
    }
}