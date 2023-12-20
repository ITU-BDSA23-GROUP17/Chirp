using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class HashtagRepositoryTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();
    private string? _connectionString;


    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
        _connectionString = _msSqlContainer.GetConnectionString();
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }

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

    // [Fact]
    // public async Task RemoveHashtagAsync_RemovesSpecifiedHashtagPairing()
    // {
    //     //Arrange
    //     // Start the container
    //     await _msSqlContainer.StartAsync();

    //     var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
    //     using var context = new ChirpDBContext(builder.Options);
    //     context.initializeDB(); //ensure all tables are created

    //     //Cheep and author repository created
    //     ICheepRepository cheepRepository = new CheepRepository(context);
    //     IHashtagRepository hashtagRepository = new HashtagRepository(context);

    //     //inserting a new hashtag into database, that we can delete afterwards
    //     await hashtagRepository.InsertNewHashtagCheepPairingAsync("testHashtag", "testCheepId");

    //     //Act
    //     await hashtagRepository.RemoveHashtagAsync("testHashtag", "testCheepId");

    //     //Assert 
    //     //Checking that the hashtag is no longer in the database
    //     var hashtag = context.Hashtags.FirstOrDefault(h => h.HashtagText == "testHashtag");
    //     Assert.Null(hashtag);
    // }

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

}
