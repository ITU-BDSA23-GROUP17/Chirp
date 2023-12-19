using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class CheepRepositoryTest : IAsyncLifetime
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
    public async Task InsertCheepAsyncAddsCheepToDatabase()
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
        await cheepRepository.InsertCheepAsync(cheepDto);
        context.SaveChanges(); // Save changes to in-memory database

        // Assert
        var insertedCheep = context.Cheeps.FirstOrDefault(c => c.Text == "test message cheep");
        Assert.NotNull(insertedCheep); // Check that we get a cheep
        Assert.Equal("test message cheep", insertedCheep.Text);
    }
}