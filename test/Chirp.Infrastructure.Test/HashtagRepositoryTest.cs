using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class HashtagRepositoryTest : IAsyncLifetime
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
    public async Task GetCheepIDsByHashtagText_ReturnsCorrectCheepIDs()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); // Make sure to replace with actual initialization if required.
        HashtagRepository hashtagRepository = new HashtagRepository(context);

        // Insert an author to associate with the cheep
        var authorId = Guid.NewGuid().ToString();
        var author = new Author { AuthorId = authorId, Name = "Test Author", Email = "author@example.com" };
        await context.Authors.AddAsync(author);

        // Insert a hashtag and cheep
        var cheepId = Guid.NewGuid().ToString();
        var hashtagText = "#test";
        var cheep = new Cheep { CheepId = cheepId, Text = "Cheep with hashtag test", AuthorId = authorId, Author = author };
        await context.Cheeps.AddAsync(cheep);
        await hashtagRepository.InsertNewHashtagCheepPairingAsync(hashtagText, cheepId);
        await context.SaveChangesAsync();

        // Act
        var cheepIds = hashtagRepository.GetCheepIDsByHashtagText(hashtagText);

        // Assert
        Assert.Contains(cheepId, cheepIds);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }


}