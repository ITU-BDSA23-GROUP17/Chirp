using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class HashtagTextRepositoryTest : IAsyncLifetime
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

        // List should contain all unique hashtags
        await context.SaveChangesAsync();
        var expectedHashtags = new List<string> { "testHashtag", "testHashtag2", "testHashtag3", "testHashtag4", "testHashtag5" };
        Assert.Equal(expectedHashtags.Count, uniqueHashtagTexts.Count);
        foreach (var tag in expectedHashtags)
        {
            Assert.Contains(tag, uniqueHashtagTexts);
        }

        // Cleanup
        context.HashtagTexts.RemoveRange(context.HashtagTexts);
        await _msSqlContainer.StopAsync();
    }
}
