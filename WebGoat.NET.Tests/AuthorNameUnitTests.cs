using System;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class AuthorNameUnitTests
    {
        [Fact]
        public void GivenValidName_WhenCreatedConstructor_ThenTrimsAndSetsValue()
        {
            // Given
            var raw = "  Alice Andersen  ";

            // When
            var name = new AuthorName(raw);

            // Then
            Assert.Equal("Alice Andersen", name.Value);
            Assert.Equal("Alice Andersen", name.ToString());
        }

        [Fact]
        public void GivenTooLongName_WhenCreatedConstructor_ThenThrowsArgumentException()
        {
            // Given
            var tooLong = new string('A', AuthorName.MaxLength + 1);

            // When + Then
            Assert.Throws<ArgumentException>(() => new AuthorName(tooLong));
        }

        [Fact]
        public void GivenValidInstance_WhenCalledToString_ThenReturnsUnderlyingValue()
        {
            // Given
            var name = new AuthorName("Bob");

            // When
            var text = name.ToString();

            // Then
            Assert.Equal("Bob", text);
        }
    }
}