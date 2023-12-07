using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public class AuthorDTOTests
{

    [Fact]
    public void AuthorDTO_ShouldHoldProvidedValues()
    {
        // Arrange
        var id = "cheep123";
        var message = "This is a test cheep";
        var timeStamp = DateTime.UtcNow;
        var authorName = "Jane Doe";
        var authorId = "author456";
        var authorImage = "image_url";

        var cheep = new CheepDTO(id, message, timeStamp, authorName, authorId, authorImage);

        // Act
        // No action required

        // Assert
        Assert.Equal(id, cheep.Id);
        Assert.Equal(message, cheep.Message);
        Assert.Equal(timeStamp, cheep.TimeStamp);
        Assert.Equal(authorName, cheep.AuthorName);
        Assert.Equal(authorId, cheep.AuthorId);
        Assert.Equal(authorImage, cheep.AuthorImage);
    }
}