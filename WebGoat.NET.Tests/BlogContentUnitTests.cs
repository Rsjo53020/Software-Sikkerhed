using System;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class BlogContentUnitTests
    {
        [Fact]
        public void GivenValidContent_WhenCreatedConstructor_ThenSetsValue()
        {
            // Given
            var text = "Det her er et helt gyldigt blogindlæg (med lidt tegn)!";

            // When
            var content = new BlogContent(text);

            // Then
            Assert.Equal(text, content.Value);
            Assert.Equal(text, content.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GivenNullOrWhitespace_WhenCreatedConstructor_ThenThrowsArgumentException(string? value)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new BlogContent(value!));
        }

        [Fact]
        public void GivenTooShortContent_WhenCreatedConstructor_ThenThrowsArgumentException()
        {
            // Given
            var tooShort = "Hej"; // < 5 tegn

            // When + Then
            Assert.Throws<ArgumentException>(() => new BlogContent(tooShort));
        }

        [Fact]
        public void GivenTooLongContent_WhenCreatedConstructor_ThenThrowsArgumentException()
        {
            // Given
            var tooLong = new string('A', 2001);

            // When + Then
            Assert.Throws<ArgumentException>(() => new BlogContent(tooLong));
        }

        [Theory]
        [InlineData("Ugyldigt tegn: #")]
        [InlineData("HTML <tag> er ikke tilladt")]
        [InlineData("Emoji 😊 er ikke tilladt")]
        public void GivenContentWithIllegalCharacters_WhenCreatedConstructor_ThenThrowsArgumentException(string invalid)
        {
            // Given + When + Then
            Assert.Throws<ArgumentException>(() => new BlogContent(invalid));
        }
    }
}