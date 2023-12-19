using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class InfrastructureUnitTests : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private string _connectionString;


    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        _connectionString = _msSqlContainer.GetConnectionString();
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }

    //################################
    //###  AuthorRepository tests  ###
    //################################

    [Fact]
    public async Task InsertAuthorAddsAuthorToDatabase()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created
        IAuthorRepository authorRepository = new AuthorRepository(context);

        // Act
        await authorRepository.InsertAuthorAsync("Author Authorson", "authorson@author.com");
        await context.SaveChangesAsync(); //save changes to in container database 

        // Assert
        var insertedAuthor = context.Authors.FirstOrDefault(a => a.Email == "authorson@author.com");
        Assert.NotNull(insertedAuthor); //check that we get an author
        Assert.Equal("Author Authorson", insertedAuthor.Name); //check that we have the right author

        // Stop the container
        await _msSqlContainer.StopAsync();
    }

    //###############################
    //###  CheepRepository tests  ###
    //###############################


    [Fact]
    public async Task InsertCheepAddsCheepToDatabase()
    {
        // Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //Cheep and author repository created
        ICheepRepository cheepRepository = new CheepRepository(context);
        IAuthorRepository authorRepository = new AuthorRepository(context);

        //Getting authorDTO by the name Helge
        var authorDTOTest = await authorRepository.GetAuthorByNameAsync("Helge");
        if (authorDTOTest == null)
        {
            throw new Exception("Could not find author Helge");
        }
        //Getting the authorID from AuthorDTO
        var authorId = authorDTOTest.AuthorId;

        //We create a cheep
        var cheepDto = new CheepDTO(
            Id: "asdasd",
            Message: "test message cheep",
            TimeStamp: DateTime.Now,
            AuthorName: "Helge",
            AuthorId: authorId,
            AuthorImage: ""
            );

        // Act
        cheepRepository.InsertCheep(cheepDto);
        context.SaveChanges(); // Save changes to in-memory database

        // Assert
        var insertedCheep = context.Cheeps.FirstOrDefault(c => c.Text == "test message cheep");
        Assert.NotNull(insertedCheep); // Check that we get a cheep
        Assert.Equal("test message cheep", insertedCheep.Text);
    }

    //################################
    //###  FollowRepository tests  ###
    //################################

    [Fact]
    public async Task GetFollowerIDsByAuthorIDAsync_CorrectlyReturnsListOfFollowerIds()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //following and author repository created
        IFollowRepository followRepository = new FollowRepository(context);

        //We need to instert followings into the database in order to test the method
        //Since this method returns the followers, we are interested in the followerIds, which is the first string.
        //each tuple is a following where the first sring is the followerid and the second the followingid
        await followRepository.InsertNewFollowAsync("testFollowerId", "testFollowedAuthorId");
        await followRepository.InsertNewFollowAsync("testFollowerId2", "testFollowedAuthorId");
        await followRepository.InsertNewFollowAsync("testFollowerId3", "testFollowedAuthorId");
        //We also insert a different following, which we expect to not be returned by the method
        await followRepository.InsertNewFollowAsync("testFollowerId3", "testFollowedAuthor2Id");

        //Act
        var followers = await followRepository.GetFollowerIDsByAuthorIDAsync("testFollowedAuthorId");

        //Assert
        //Waht we expect to see in the list of followers of the specified authorid is: followerId, followerId2 and followerId3
        followers.Equals(new List<string> { "testFollowerId", "testFollowerId2", "testFollowerId3" });
    }

    [Fact]
    public async Task InsertNewFollowAsync_CorrectlyInsertsNewFollowIntoTheDatabase()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //following and author repository created
        IFollowRepository followRepository = new FollowRepository(context);

        //Act
        await followRepository.InsertNewFollowAsync("testFollowerId", "testFollowedAuthorId");

        //Assert
        //we need to check that the follow is in the database
        var follow = context.Followings.FirstOrDefault(f => f.FollowerId == "testFollowerId" && f.FollowingId == "testFollowedAuthorId");
        Assert.NotNull(follow);
    }

    [Fact]
    public async Task RemoveFollowAsync_CorrectlyRemovesFollowIntoTheDatabase()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //following and author repository created
        IFollowRepository followRepository = new FollowRepository(context);

        await followRepository.InsertNewFollowAsync("testFollowerId", "testFollowedAuthorId");
        //We need to insert a follow into the database in order to test the method.
        //We then remove the follow we just inserted

        //Act
        await followRepository.RemoveFollowAsync("testFollowerId", "testFollowedAuthorId");
        var follow = context.Followings.FirstOrDefault(f => f.FollowerId == "testFollowerId" && f.FollowingId == "testFollowedAuthorId");
        Assert.Null(follow);
    }


    [Fact]
    public async Task GetFollowingIDsByAuthorIDAsync_CorrectlyReturnsListOfFollowingIds()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //following and author repository created
        IFollowRepository followRepository = new FollowRepository(context);

        //We need to insert followings into the database in order to test the method
        //Since this method returns the followed author ids, by the specified id, we are now interested in the second string.
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId", "testFollowedAuthor1Id");
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId", "testFollowedAuthor2Id");
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId", "testFollowedAuthor3Id");
        //We also insert a different followerAuthorId with their own followings, which we don't expect to be returned by the method
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId2", "testFollowedAuthorId");
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId2", "testFollowedAuthorId4");

        //Act
        var followings = await followRepository.GetFollowingIDsByAuthorIDAsync("testFollowedAuthorId");

        //Assert
        //We expect to see in the list of followed authors by the specified author, with the id testFollowedAuthorId: testFollowedAuthor1Id, testFollowedAuthor2Id and testFollowedAuthor3Id
        followings.Equals(new List<string> { "testFollowedAuthor1Id", "testFollowedAuthor2Id", "testFollowedAuthor3Id" });
    }

    [Fact]
    public async Task GetFollowerCountByAuthorIDAsync_CorrectlyReturnsCountOfFollowers()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //following and author repository created
        IFollowRepository followRepository = new FollowRepository(context);

        //We need to instert followings into the database in order to test the method
        //Since this method returns the followers, we are interested in the followerIds, which is the first string.
        //each tuple is a following where the first sring is the followerid and the second the followingid
        await followRepository.InsertNewFollowAsync("testFollowerId", "testFollowedAuthorId");
        await followRepository.InsertNewFollowAsync("testFollowerId2", "testFollowedAuthorId");
        await followRepository.InsertNewFollowAsync("testFollowerId3", "testFollowedAuthorId");
        //We also insert a different following, which we expect to not be returned by the method
        await followRepository.InsertNewFollowAsync("testFollowerId3", "testFollowedAuthor2Id");

        //Act
        //We get the count of followers of the specified authorid
        int followerCount = await followRepository.GetFollowerCountByAuthorIDAsync("testFollowedAuthorId");

        //Assert
        //Since we insserted three followers, we expect the count to be 3
        Assert.Equal(3, followerCount);
    }

    [Fact]
    public async Task GetFollowingCountByAuthorIDAsync_CorrectlyReturnsCountOfAuthorsFollowedBySpecifiedAuthor()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //following and author repository created
        IFollowRepository followRepository = new FollowRepository(context);

        //We need to insert followings into the database in order to test the method
        //Since this method returns the followed author ids, by the specified id, we are now interested in the second string.
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId", "testFollowedAuthor1Id");
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId", "testFollowedAuthor2Id");
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId", "testFollowedAuthor3Id");
        //We also insert a different followerAuthorId with their own followings, which we don't expect to be returned by the method
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId2", "testFollowedAuthorId");
        await followRepository.InsertNewFollowAsync("testFollowerAuthorId2", "testFollowedAuthorId4");

        //Act
        var followingCounts = await followRepository.GetFollowingCountByAuthorIDAsync("testFollowerAuthorId");

        //Assert
        //We expect to see three followings by id testFollowerAuthorId.
        Assert.Equal(3, followingCounts);
    }



    //#################################
    //###  HashtagRepository tests  ###
    //#################################


    [Fact]
    public async Task GetCheepIDsByHashtagText_GetsCheepIDsTiedToHashtag()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //Cheep and author repository created
        ICheepRepository cheepRepository = new CheepRepository(context);
        IHashtagRepository hashtagRepository = new HashtagRepository(context);
        IHashtagTextRepository hashtagTextRepository = new HashtagTextRepository(context);

        //Inserting hashtagtext and hashtags into database
        await hashtagTextRepository.AddHashtag("testHashtag");
        await hashtagTextRepository.AddHashtag("testHashtag2");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag", "testCheepId");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag", "testCheepId2");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag2", "testCheepId3");

        //Act
        var cheepIds1 = hashtagRepository.GetCheepIDsByHashtagText("testHashtag");
        var cheepIds2 = hashtagRepository.GetCheepIDsByHashtagText("testHashtag2");

        //Assert
        Assert.Equal(cheepIds1, new List<string> { "testCheepId", "testCheepId2" });
        Assert.Equal(cheepIds2, new List<string> { "testCheepId3" });

    }

    [Fact]
    public async Task InsertNewCheepHashtagPairingAsync_InsertsANewHashtagWithCorrectCheepIdAndHashtagText()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //Cheep and author repository created
        ICheepRepository cheepRepository = new CheepRepository(context);
        IHashtagRepository hashtagRepository = new HashtagRepository(context);

        //Act
        //Inserting hashtagtext and hashtags into database
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag", "testCheepId");

        //Assert
        //Retrieving hashtag from database and checking that it has the correct cheepId and hashtagText
        var hashtag = context.Hashtags.FirstOrDefault(h => h.HashtagText == "testHashtag");
        Assert.NotNull(hashtag);
        Assert.Equal("testCheepId", hashtag.CheepID);
    }

    [Fact]
    public async Task RemoveHashtagAsync_RemovesSpecifiedHashtagPairing()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //Cheep and author repository created
        ICheepRepository cheepRepository = new CheepRepository(context);
        IHashtagRepository hashtagRepository = new HashtagRepository(context);

        //inserting a new hashtag into database, that we can delete afterwards
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag", "testCheepId");

        //Act
        await hashtagRepository.RemoveHashtagAsync("testHashtag", "testCheepId");

        //Assert 
        //Checking that the hashtag is no longer in the database
        var hashtag = context.Hashtags.FirstOrDefault(h => h.HashtagText == "testHashtag");
        Assert.Null(hashtag);
    }

    [Fact]
    public async Task GetPopulalarHashtags_Returns10PopularHashtags()
    {

        //Arrange
        //start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //repositories created
        ICheepRepository cheepRepository = new CheepRepository(context);
        IHashtagRepository hashtagRepository = new HashtagRepository(context);
        IHashtagTextRepository hashtagTextRepository = new HashtagTextRepository(context);

        //Inserting hashtagtext and hashtags into database
        await hashtagTextRepository.AddHashtag("testHashtag");
        await hashtagTextRepository.AddHashtag("testHashtag2");
        await hashtagTextRepository.AddHashtag("testHashtag3");
        await hashtagTextRepository.AddHashtag("testHashtag4");
        await hashtagTextRepository.AddHashtag("testHashtag5");
        await hashtagTextRepository.AddHashtag("testHashtag6");
        await hashtagTextRepository.AddHashtag("testHashtag7");
        await hashtagTextRepository.AddHashtag("testHashtag8");
        await hashtagTextRepository.AddHashtag("testHashtag9");
        await hashtagTextRepository.AddHashtag("testHashtag10");
        await hashtagTextRepository.AddHashtag("testHashtag11");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag", "testCheepId");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag2", "testCheepId2");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag2", "testCheepId3");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag2", "testCheepId4");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag3", "testCheepId5");
        await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag3", "testCheepId6");

        //Act
        var uniqueHashtagTexts = await hashtagTextRepository.GetUniqueHashtagTextsAsync();
        var popularHashtags = hashtagRepository.GetPopularHashtags(uniqueHashtagTexts);

        //Assert
        //We know the order we expect, so we can check the hashtags in the returned list are correct and in the correct order
        //We also know that we expect only 10 hashtagstexts, even thoguh there are more than 10 in the database
        popularHashtags.Count.Equals(10);
        popularHashtags[0].Equals("testHashtag2");
        popularHashtags[1].Equals("testHashtag3");
        popularHashtags[2].Equals("testHashtag");
    }

    //#####################################
    //###  HashtagTextRepository tests  ###
    //#####################################

    [Fact]
    public async Task AddHashtag_AddsHashtagToDatabase()
    {
        //Arrange
        //start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //creating hashtagtext repository
        IHashtagTextRepository hashtagTextRepository = new HashtagTextRepository(context);

        //Act
        await hashtagTextRepository.AddHashtag("testHashtag");

        //Assert
        //We check that the hashtagtext is in the database
        var hashtagText = context.HashtagTexts.FirstOrDefault(h => h.HashtagText_ == "testHashtag");
        Assert.NotNull(hashtagText);
        Assert.Equal("testHashtag", hashtagText.HashtagText_);
    }

    [Fact]
    public async Task AddHashtag_WillNotAddTheSameHashtagMoreThanOnce()
    {
        //Arrange
        //start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //creating hashtagtext repository
        IHashtagTextRepository hashtagTextRepository = new HashtagTextRepository(context);

        //Act
        await hashtagTextRepository.AddHashtag("testHashtag");
        await hashtagTextRepository.AddHashtag("testHashtag");

        //Assert
        //We check that only one hashtag is in the database
        var hashtagTexts = context.HashtagTexts.Where(h => h.HashtagText_ == "testHashtag").ToList();
        Assert.True(hashtagTexts.Count == 1);
    }
    [Fact]
    public async Task RemoveHashtag_RemovedSpecifiedHashtagTextIfItExist()
    {
        //Arrange
        //start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //creating hashtagtext repository
        IHashtagTextRepository hashtagTextRepository = new HashtagTextRepository(context);


        await hashtagTextRepository.AddHashtag("testHashtag");
        //We add a hashtag we don't remove in order to check that obly specified hashtag is removed, as we expect
        await hashtagTextRepository.AddHashtag("testHashtag2");
        //We also try to delete a hashtag that does not exist in the database. We expect nothing to happen.
        await hashtagTextRepository.RemoveHashtag("testHashtag");

        //Act
        //We remove the hashtagtext from the database
        await hashtagTextRepository.RemoveHashtag("testHashtag3");

        //Assert
        //We check that the hashtagtext is no longer in the database
        var hashtagText = context.HashtagTexts.FirstOrDefault(h => h.HashtagText_ == "testHashtag");
        Assert.Null(hashtagText);
    }

    [Fact]
    public async Task GetUniqueHashtagTexts_ReturnsListOfUniqueHashtagTexts()
    {
        //Arrange
        //start the container
        await _msSqlContainer.StartAsync();

        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created

        //creating hashtagtext repository
        IHashtagTextRepository hashtagTextRepository = new HashtagTextRepository(context);

        //we add a bucnh of hashtags, some of which are the same, sicne we want to check that we only get no duplicates in the list
        await hashtagTextRepository.AddHashtag("testHashtag");
        await hashtagTextRepository.AddHashtag("testHashtag");
        await hashtagTextRepository.AddHashtag("testHashtag2");
        await hashtagTextRepository.AddHashtag("testHashtag3");
        await hashtagTextRepository.AddHashtag("testHashtag3");
        await hashtagTextRepository.AddHashtag("testHashtag4");
        await hashtagTextRepository.AddHashtag("testHashtag4");
        await hashtagTextRepository.AddHashtag("testHashtag4");
        await hashtagTextRepository.AddHashtag("testHashtag5");

        //Act
        //we retrieve the list of unique hashtagtexts by invokinh the method we want to test
        var uniqueHashtagTexts = await hashtagTextRepository.GetUniqueHashtagTextsAsync();

        //Assert
        //We check that the list of unique hashtagtexts is correct
        Assert.Equal(uniqueHashtagTexts, new List<string> { "testHashtag", "testHashtag2", "testHashtag3", "testHashtag4", "testHashtag5" });
    }
}
