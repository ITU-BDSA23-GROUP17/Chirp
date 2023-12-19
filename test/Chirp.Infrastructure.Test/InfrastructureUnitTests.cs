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
        var hasInserted = cheepRepository.InsertCheep(cheepDto);

        // Assert
        Assert.False(hasInserted);
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

    [Fact]
    public async Task GetStatusNotNull()
    {
        /*
            Arrange
        */

        // Start the container
        await _msSqlContainer.StartAsync();

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

        /*
            Act
        */

        var receivedStatus = authorDTOTest.Status;

        /*
            Assert
        */

        Assert.NotNull(receivedStatus);

    }

    [Fact]
    public async Task SetUserStatusOnline()
    {
        /*
            Arrange
        */

        // Start the container
        await _msSqlContainer.StartAsync();

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

        /*
            Act
        */

        await authorRepository.UpdateAuthorStatusOnline(authorDTOTest.Email);
        var receivedStatus = await authorRepository.GetAuthorStatusAsync(authorDTOTest.Email);

        /*
            Assert
        */

        Assert.Equal("ONLINE", receivedStatus);

    }

    [Fact]
    public async Task SetUserStatusOffline()
    {
        /*
            Arrange
        */

        // Start the container
        await _msSqlContainer.StartAsync();

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

        /*
            Act
        */

        await authorRepository.UpdateAuthorStatusOffline(authorDTOTest.Email);
        var receivedStatus = await authorRepository.GetAuthorStatusAsync(authorDTOTest.Email);

        /*
            Assert
        */

        Assert.Equal("OFFLINE", receivedStatus);

    }

    [Fact]
    public async Task SetUserStatusUnavailable()
    {
        /*
            Arrange
        */

        // Start the container
        await _msSqlContainer.StartAsync();

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

        /*
            Act
        */

        await authorRepository.UpdateAuthorStatusUnavailable(authorDTOTest.Email);
        var receivedStatus = await authorRepository.GetAuthorStatusAsync(authorDTOTest.Email);

        /*
            Assert
        */

        Assert.Equal("UNAVAILABLE", receivedStatus);

    }

}
