using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class AuthorRepositoryUnitTest : IAsyncLifetime
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
    [Trait("Category", "Integration")]
    public async Task GetAuthorByEmailAsync_ReturnsCorrectAuthor()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created
        IAuthorRepository authorRepository = new AuthorRepository(context);
        var expectedEmail = "tanda@itu.dk";

        // Act
        await authorRepository.InsertAuthorAsync("tan dang", expectedEmail);
        await context.SaveChangesAsync();
        var authorDto = await authorRepository.GetAuthorByEmailAsync(expectedEmail);

        // Assert
        Assert.NotNull(authorDto);
        Assert.Equal("tan dang", authorDto.Name);
        Assert.Equal(expectedEmail, authorDto.Email);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAuthorByNameAsync_ReturnsCorrectAuthor()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created
        IAuthorRepository authorRepository = new AuthorRepository(context);
        var expectedName = "tan dang";

        // Act
        await authorRepository.InsertAuthorAsync(expectedName, "");
        await context.SaveChangesAsync();

        var authorDto = await authorRepository.GetAuthorByNameAsync(expectedName);

        // Assert
        Assert.NotNull(authorDto);
        Assert.Equal(expectedName, authorDto.Name);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAuthorByIdAsync_ReturnsCorrectAuthor()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created
        IAuthorRepository authorRepository = new AuthorRepository(context);
        var expectedName = "tan dang";
        var expectedEmail = "tanda@itu.dk";

        // Act
        await authorRepository.InsertAuthorAsync(expectedName, expectedEmail);
        await context.SaveChangesAsync();
        var insertedAuthor = await context.Authors.FirstOrDefaultAsync(a => a.Email == expectedEmail);

        AuthorDTO? authorDto = null;

        if (insertedAuthor != null)
        {
            authorDto = await authorRepository.GetAuthorByIdAsync(insertedAuthor.AuthorId);

        }

        // Assert
        Assert.NotNull(authorDto);
        Assert.Equal(expectedName, authorDto.Name);
        Assert.Equal(expectedEmail, authorDto.Email);
        Assert.Equal(insertedAuthor?.AuthorId, authorDto.AuthorId);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }

    // Test needs to be fixed
    [Fact]
    [Trait("Category", "Integration")]
    public async Task GetAuthorsByIdsAsync_ReturnsCorrectAuthors()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IAuthorRepository authorRepository = new AuthorRepository(context);

        // Insert authors
        var authorName1 = "tan dang";
        var authorEmail1 = "tanda@itu.dk";
        await authorRepository.InsertAuthorAsync(authorName1, authorEmail1);

        var authorName2 = "tan deng";
        var authorEmail2 = "tande@itu.dk";
        await authorRepository.InsertAuthorAsync(authorName2, authorEmail2);

        await context.SaveChangesAsync();

        var insertedAuthors = await context.Authors.Where(a => a.Email == authorEmail1 || a.Email == authorEmail2).ToListAsync();
        var authorIds = insertedAuthors.Select(a => a.AuthorId).ToList();

        // Act
        var authorsDtoList = await authorRepository.GetAuthorsByIdsAsync(authorIds);

        // Assert
        Assert.Equal(2, authorsDtoList.Count);
        Assert.Contains(authorsDtoList, a => a.Email == authorEmail1);
        Assert.Contains(authorsDtoList, a => a.Email == authorEmail2);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }

    // Test needs to be fixed
    [Fact]
    public async Task SendCheepAsync_AddsCheepToAuthor()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IAuthorRepository authorRepository = new AuthorRepository(context);

        // Insert an author
        var authorName = "tan dang";
        var authorEmail = "tanda@itu.dk";
        await authorRepository.InsertAuthorAsync(authorName, authorEmail);
        await context.SaveChangesAsync();

        var author = await context.Authors.FirstOrDefaultAsync(a => a.Email == authorEmail);

        // Prepare the cheep
        var message = "cheep123";

        AuthorInfoDTO? authorInfoDTO = null;
        if (author != null)
        {
            authorInfoDTO = new AuthorInfoDTO(author.AuthorId, authorName, authorEmail);
        }

        // Act
        CheepDTO? cheepDto = null;
        if (authorInfoDTO != null)
        {
            cheepDto = await authorRepository.SendCheepAsync(message, authorInfoDTO);
        }
        await context.SaveChangesAsync();

        // Assert
        Cheep? cheep = null;
        if (cheepDto != null)
        {
            cheep = await context.Cheeps.FindAsync(cheepDto.Id);

        }
        Assert.NotNull(cheep);
        Assert.Equal(message, cheep.Text);
        if (author != null)
        {

            Assert.Equal(author.AuthorId, cheep.AuthorId);
        }

        // Stop the container
        await _msSqlContainer.StopAsync();
    }


    [Fact]
    [Trait("Category", "Integration")]
    public async Task UpdateAuthorAsync_UpdatesAuthorDetailsCorrectly()
    {
        // Start the container
        await _msSqlContainer.StartAsync();

        // Arrange
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB(); //ensure all tables are created
        IAuthorRepository authorRepository = new AuthorRepository(context);
        var originalName = "tan dang";
        var originalEmail = "tanda@itu.dk";
        var originalStatus = "ONLINE";

        await authorRepository.InsertAuthorAsync(originalName, originalEmail);
        await context.SaveChangesAsync();
        var insertedAuthor = await context.Authors.FirstOrDefaultAsync(a => a.Email == originalEmail);

        List<CheepDTO>? cheepsList = insertedAuthor?.Cheeps.Select(c => new CheepDTO(c.CheepId, c.Text, c.TimeStamp, c.Author.Name, c.Author.AuthorId, c.Author.Image)).ToList();

        // Update details
        var updatedName = "tan deng";
        var updatedEmail = "tande@itu.dk";
        AuthorDTO? authorToUpdate = null;
        if (insertedAuthor != null)
        {
            if (cheepsList != null){
                authorToUpdate = new AuthorDTO(insertedAuthor.AuthorId, updatedName, updatedEmail, originalStatus, cheepsList, insertedAuthor.Image);
            } else {
                authorToUpdate = new AuthorDTO(insertedAuthor.AuthorId, updatedName, updatedEmail, originalStatus, new List<CheepDTO> {}, insertedAuthor.Image);
            }
        }

        // Act
        if (authorToUpdate != null)
        {

            await authorRepository.UpdateAuthorAsync(authorToUpdate);
            await context.SaveChangesAsync();
        }

        // Retrieve updated author
        var updatedAuthor = await context.Authors.FindAsync(insertedAuthor?.AuthorId);

        // Assert
        Assert.NotNull(updatedAuthor);
        Assert.Equal(updatedName, updatedAuthor.Name);
        Assert.Equal(updatedEmail, updatedAuthor.Email);

        // Stop the container
        await _msSqlContainer.StopAsync();
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
    public async Task GetStatusIsValid()
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
        var isValidStatus = false;
        if (receivedStatus.Equals("ONLINE") || receivedStatus.Equals("OFFLINE") || receivedStatus.Equals("UNAVAILABLE")){
            isValidStatus = true;
        }

        /*
            Assert
        */

        Assert.True(isValidStatus);

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
        var receivedStatus = authorDTOTest.Status;

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

        await authorRepository.UpdateAuthorStatusOnline(authorDTOTest.Email);
        var receivedStatus = authorDTOTest.Status;

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

        await authorRepository.UpdateAuthorStatusOnline(authorDTOTest.Email);
        var receivedStatus = authorDTOTest.Status;

        /*
            Assert
        */

        Assert.Equal("UNAVAILABLE", receivedStatus);

    }


    // [Fact]
    // public async Task DeleteAuthorAsync_DeletesAuthorSuccessfully()
    // {
    //     // Start the container
    //     await _msSqlContainer.StartAsync();

    //     // Arrange
    //     var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
    //     using var context = new ChirpDBContext(builder.Options);
    //     context.initializeDB();
    //     IAuthorRepository authorRepository = new AuthorRepository(context);
    //     var authorName = "tan dang";
    //     var authorEmail = "tanda@itu.dk";

    //     await authorRepository.InsertAuthorAsync(authorName, authorEmail);
    //     await context.SaveChangesAsync();

    //     var insertedAuthor = await context.Authors.FirstOrDefaultAsync(a => a.Email == authorEmail);

    //     // Act
    //     await authorRepository.DeleteAuthorAsync(int.Parse(insertedAuthor.AuthorId));
    //     await context.SaveChangesAsync();

    //     var deletedAuthor = await context.Authors.FindAsync(insertedAuthor.AuthorId);

    //     // Assert
    //     Assert.Null(deletedAuthor);

    //     // Stop the container
    //     await _msSqlContainer.StopAsync();
    // }
}