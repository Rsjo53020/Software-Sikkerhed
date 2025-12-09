using System;
using WebGoatCore.Models;
using Xunit;

namespace WebGoat.NET.Tests
{
    public class BlogResponseDMUnitTests
    {
        [Fact]
        public void GivenValidArguments_WhenCreatedConstructor_ThenPropertiesAreSet()
        {
            // Given
            var blogEntryId = 42;
            var responseDate = new DateTime(2025, 1, 1);
            var author = new AuthorName("Alice");
            var contents = new BlogContent("Det her er et helt validt blog-svar!");

            // When
            var response = new BlogResponseDM(blogEntryId, responseDate, author, contents);

            // Then
            Assert.Equal(0, response.Id); // Id er ikke sat i ctor og vil være 0 (default int)
            Assert.Equal(blogEntryId, response.BlogEntryId);
            Assert.Equal(responseDate, response.ResponseDate);
            Assert.Same(author, response.Author);
            Assert.Same(contents, response.Contents);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void GivenNonPositiveBlogEntryId_WhenCreatedConstructor_ThenThrowsArgumentOutOfRangeException(int invalidId)
        {
            // Given
            var responseDate = new DateTime(2025, 1, 1);
            var author = new AuthorName("Alice");
            var contents = new BlogContent("Gyldigt blogindhold");

            // When + Then
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new BlogResponseDM(invalidId, responseDate, author, contents));
        }

        [Fact]
        public void GivenDefaultResponseDate_WhenCreatedConstructor_ThenThrowsArgumentException()
        {
            // Given
            var author = new AuthorName("Alice");
            var contents = new BlogContent("Gyldigt blogindhold");

            // When + Then
            Assert.Throws<ArgumentException>(() =>
                new BlogResponseDM(1, default, author, contents));
        }

        [Fact]
        public void GivenNullAuthor_WhenCreatedConstructor_ThenThrowsArgumentNullException()
        {
            // Given
            var responseDate = new DateTime(2025, 1, 1);
            var contents = new BlogContent("Gyldigt blogindhold");

            // When + Then
            Assert.Throws<ArgumentNullException>(() =>
                new BlogResponseDM(1, responseDate, null!, contents));
        }

        [Fact]
        public void GivenNullContents_WhenCreatedConstructor_ThenThrowsArgumentNullException()
        {
            // Given
            var responseDate = new DateTime(2025, 1, 1);
            var author = new AuthorName("Alice");

            // When + Then
            Assert.Throws<ArgumentNullException>(() =>
                new BlogResponseDM(1, responseDate, author, null!));
        }
    }
}