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
            AuthorId: authorId
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
    public async Task CheepOverCharacterLimitNotAllowed()
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

        string cheepOverCharLimit = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi iaculis libero eget risus ullamcorper eleifend. Nunc ac egestas nulla. Ut mattis luctus aliquet. Donec at nisl vulputate, laoreet diam a, maximus lectus. 
        Ut massa justo, bibendum at orci a, imperdiet vulputate tortor. Etiam mauris nibh, posuere et pharetra eu, porttitor vitae ex. Curabitur ultricies nulla sed augue malesuada, ut tincidunt sapien placerat. 
        Suspendisse elementum ante at elementum egestas. Integer condimentum vestibulum risus, et mollis lectus placerat vel. Sed interdum quam sem, non aliquet magna iaculis id. Duis porta odio ut nunc porttitor, mattis imperdiet dolor pulvinar. 
        Morbi ac neque magna. Suspendisse tincidunt ligula vel molestie facilisis. Vivamus tincidunt dolor nibh, eget iaculis tellus eleifend ac. Praesent at lobortis massa, in dictum ligula. Morbi rutrum sem sed ullamcorper venenatis.
        Quisque faucibus vehicula risus, quis fermentum mi condimentum nec. Interdum et malesuada fames ac ante ipsum primis in faucibus. Cras tincidunt nisi dolor, in iaculis mi fermentum at. Praesent sed nunc non turpis sagittis tristique condimentum in ante. 
        Sed cursus lectus eget nisl consequat maximus. Donec pharetra congue dolor a consectetur. Nunc consectetur arcu eu fermentum eleifend. Proin ac massa quis nisi suscipit dignissim. 
        Orci varius natoque penatibus et magnis dis parturient.";

        //We create a cheep (over 160 characters)
        var longCheepDto = new CheepDTO(
            Id: "asdasd",
            Message: cheepOverCharLimit,
            TimeStamp: DateTime.Now,
            AuthorName: "Helge",
            AuthorId: authorId
            );

        // Act
        var longCheepInsertionError = Assert.Throws<Exception>(() => {
            cheepRepository.InsertCheep(longCheepDto);
            context.SaveChanges(); // Save changes to in-memory database
        });

        // Assert
        // Assert
        var notInsertedCheep = context.Cheeps.FirstOrDefault(c => c.Text == cheepOverCharLimit);
        Assert.Equal("Cheep length is too long", longCheepInsertionError.Message);
        Assert.Null(notInsertedCheep); 

    }

    [Fact]
    public async Task EmptyCheepNotAllowed()
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

        string emptyCheep = @"";

        //We create a cheep (over 160 characters)
        var emptyCheepDto = new CheepDTO(
            Id: "asdasd",
            Message: emptyCheep,
            TimeStamp: DateTime.Now,
            AuthorName: "Helge",
            AuthorId: authorId
            );

        // Act
        var emptyCheepInsertionError = Assert.Throws<Exception>(() => {
            cheepRepository.InsertCheep(emptyCheepDto);
            context.SaveChanges(); // Save changes to in-memory database
        });

        // Assert
        var notInsertedCheep = context.Cheeps.FirstOrDefault(c => c.Text == "");
        Assert.Equal("Cheep is empty", emptyCheepInsertionError.Message);
        Assert.Null(notInsertedCheep); 

    }

    [Fact]
    public async Task getUserOnlineStatus(){
         // Arrange
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


        // Act
        var statusOnline = await authorRepository.GetStatusByAuthorID(authorId);
        

        // Assert
        Assert.Equal("ONLINE", statusOnline?.Status);
    }
}