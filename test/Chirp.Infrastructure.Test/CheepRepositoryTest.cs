using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class CheepRepositoryTest : IAsyncLifetime
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
    [Trait("Category", "Integration")]
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

    
    [Fact]
    public async Task CheepOverLimitNotInserted(){
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

        // The cheep that is too big
        var largeMessage = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus libero nunc, 
        fringilla sit amet leo ac, pretium pellentesque diam. Nulla fringilla eros ac faucibus vehicula. 
        Quisque vel diam nulla.";

        //We create a cheep
        var cheepDto = new CheepDTO(
            Id: "asdasd123",
            Message: largeMessage,
            TimeStamp: DateTime.Now,
            AuthorName: "Helge",
            AuthorId: authorId,
            AuthorImage: ""
            );

        // Act
        var hasInserted = await cheepRepository.InsertCheepAsync(cheepDto);
        var retreivedCheep = await cheepRepository.GetCheepByIDAsync(cheepDto.Id);

        // Assert
        Assert.False(hasInserted);
        Assert.Null(retreivedCheep);
    }

    [Fact]
    public async Task CheepUnderLimitNotInserted(){
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
            Id: "asdasd123",
            Message: "", // empty cheep
            TimeStamp: DateTime.Now,
            AuthorName: "Helge",
            AuthorId: authorId,
            AuthorImage: ""
            );

        // Act
        var hasInserted = await cheepRepository.InsertCheepAsync(cheepDto);
        var retreivedCheep = await cheepRepository.GetCheepByIDAsync(cheepDto.Id);

        // Assert
        Assert.False(hasInserted);
        Assert.Null(retreivedCheep);
    }


    [Fact]
    public async Task GetCheepIDsByHashtagTextGetsCheepIDsTiedToHashtag()
    {
        //Arrange
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
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

}