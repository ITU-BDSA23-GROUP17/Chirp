using Chirp.Core;

public class DTOTests
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
    [Fact]
    public void ReactionDTO_Should()
    {
        // Arrange
        var cheepId = "123456";
        var authorId = "author123";
        var timeStamp = DateTime.UtcNow;
        var reactionTypeId = "like";

        // Act
        var reactionDTO = new ReactionDTO(cheepId, authorId, timeStamp, reactionTypeId);

        // Assert
        Assert.Equal(cheepId, reactionDTO.CheepId);
        Assert.Equal(authorId, reactionDTO.AuthorId);
        Assert.Equal(timeStamp, reactionDTO.TimeStamp);
    }
}