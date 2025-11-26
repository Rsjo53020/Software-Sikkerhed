using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class CompanyNameUnitTests
    {
        [Fact]
        public void GivenValidName_WhenConstructing_ThenReturnsTrimmedInstance()
        {
            // Given
            var input = "  Acme Corp  ";

            // When
            var result = new CompanyName(input);

            // Then
            Assert.Equal("Acme Corp", result.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrEmpty_WhenConstructing_ThenThrowsArgumentExceptionn(string? value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new CompanyName(value!));
        }

        [Fact]
        public void  GivenNameExceedingMaxLength_WhenConstructing_ThenThrowsArgumentException()
        {
              // Given
            var newCompanyName = new string('A', 101);

            // When + Then
            Assert.Throws<ArgumentException>(() => new CompanyName(newCompanyName));
        }
    }
}