using Chirp.Core;


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

        // Act
        var cheep = new CheepDTO(id, message, timeStamp, authorName, authorId, authorImage);

        // Assert
        Assert.Equal(id, cheep.Id);
        Assert.Equal(message, cheep.Message);
        Assert.Equal(timeStamp, cheep.TimeStamp);
        Assert.Equal(authorName, cheep.AuthorName);
        Assert.Equal(authorId, cheep.AuthorId);
        Assert.Equal(authorImage, cheep.AuthorImage);
    }
    [Fact]
    public void FollowDTO_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var followerId = "follower123";
        var followingId = "following456";
        var timestamp = DateTime.UtcNow;

        // Act
        var followDTO = new FollowDTO(followerId, followingId, timestamp);

        // Assert
        Assert.Equal(followerId, followDTO.FollowerId);
        Assert.Equal(followingId, followDTO.FollowingId);
        Assert.Equal(timestamp, followDTO.Timestamp);
    }
}