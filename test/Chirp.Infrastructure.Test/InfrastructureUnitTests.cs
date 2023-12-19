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
    public async Task GetAuthorByEmailAsync()
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
    public async Task GetAuthorByNameAsync()
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

        var authorDto = await authorRepository.GetAuthorByIdAsync(insertedAuthor.AuthorId);

        // Assert
        Assert.NotNull(authorDto);
        Assert.Equal(expectedName, authorDto.Name);
        Assert.Equal(expectedEmail, authorDto.Email);
        Assert.Equal(insertedAuthor.AuthorId, authorDto.AuthorId);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }

    [Fact]
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
        var authorInfoDTO = new AuthorInfoDTO(authorName, author.AuthorId, "");

        // Act
        var cheepDto = await authorRepository.SendCheepAsync(message, authorInfoDTO);
        await context.SaveChangesAsync();

        // Assert
        var cheep = await context.Cheeps.FindAsync(cheepDto.Id);
        Assert.NotNull(cheep);
        Assert.Equal(message, cheep.Text);
        Assert.Equal(author.AuthorId, cheep.AuthorId);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }


    [Fact]
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

        await authorRepository.InsertAuthorAsync(originalName, originalEmail);
        await context.SaveChangesAsync();
        var insertedAuthor = await context.Authors.FirstOrDefaultAsync(a => a.Email == originalEmail);

        // Update details
        var updatedName = "tan deng";
        var updatedEmail = "tande@itu.dk";
        var authorToUpdate = new AuthorDTO(insertedAuthor.AuthorId, updatedName, updatedEmail, null, insertedAuthor.Image);

        // Act
        await authorRepository.UpdateAuthorAsync(authorToUpdate);
        await context.SaveChangesAsync();

        // Retrieve updated author
        var updatedAuthor = await context.Authors.FindAsync(insertedAuthor.AuthorId);

        // Assert
        Assert.NotNull(updatedAuthor);
        Assert.Equal(updatedName, updatedAuthor.Name);
        Assert.Equal(updatedEmail, updatedAuthor.Email);

        // Stop the container
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task GetFollowerIDsByAuthorIDAsync_ReturnsCorrectFollowerIDs()
    {
        // Arrange
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IFollowRepository followRepository = new FollowRepository(context);
        var followerId = Guid.NewGuid().ToString();
        var followingId = Guid.NewGuid().ToString();

        // Act
        await followRepository.InsertNewFollowAsync(followerId, followingId);
        await context.SaveChangesAsync();
        var followerIDs = await followRepository.GetFollowerIDsByAuthorIDAsync(followingId);

        // Assert
        Assert.Contains(followerId, followerIDs);

        // Cleanup
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task GetFollowingIDsByAuthorIDAsync_ReturnsCorrectFollowingIDs()
    {
        // Arrange
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IFollowRepository followRepository = new FollowRepository(context);
        var followerId = Guid.NewGuid().ToString();
        var followingId = Guid.NewGuid().ToString();

        // Act
        await followRepository.InsertNewFollowAsync(followerId, followingId);
        await context.SaveChangesAsync();
        var followingIDs = await followRepository.GetFollowingIDsByAuthorIDAsync(followerId);

        // Assert
        Assert.Contains(followingId, followingIDs);

        // Cleanup
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task InsertNewFollowAsync_InsertsFollowSuccessfully()
    {
        // Arrange
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IFollowRepository followRepository = new FollowRepository(context);
        var followerId = Guid.NewGuid().ToString();
        var followingId = Guid.NewGuid().ToString();

        // Act
        await followRepository.InsertNewFollowAsync(followerId, followingId);
        await context.SaveChangesAsync();
        var follow = await context.Followings.FindAsync(followerId, followingId);

        // Assert
        Assert.NotNull(follow);

        // Cleanup
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task RemoveFollowAsync_RemovesFollowSuccessfully()
    {
        // Arrange
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IFollowRepository followRepository = new FollowRepository(context);
        var followerId = Guid.NewGuid().ToString();
        var followingId = Guid.NewGuid().ToString();
        await followRepository.InsertNewFollowAsync(followerId, followingId);
        await context.SaveChangesAsync();

        // Act
        await followRepository.RemoveFollowAsync(followerId, followingId);
        await context.SaveChangesAsync();
        var follow = await context.Followings.FindAsync(followerId, followingId);

        // Assert
        Assert.Null(follow);

        // Cleanup
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task GetFollowerCountByAuthorIDAsync_ReturnsCorrectCount()
    {
        // Arrange
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IFollowRepository followRepository = new FollowRepository(context);
        var authorId = Guid.NewGuid().ToString();
        var followerId1 = Guid.NewGuid().ToString();
        var followerId2 = Guid.NewGuid().ToString();

        // Insert followers
        await followRepository.InsertNewFollowAsync(followerId1, authorId);
        await followRepository.InsertNewFollowAsync(followerId2, authorId);
        await context.SaveChangesAsync();

        // Act
        var followerCount = await followRepository.GetFollowerCountByAuthorIDAsync(authorId);

        // Assert
        Assert.Equal(2, followerCount);

        // Cleanup
        await _msSqlContainer.StopAsync();
    }

    [Fact]
    public async Task GetFollowingCountByAuthorIDAsync_ReturnsCorrectCount()
    {
        // Arrange
        await _msSqlContainer.StartAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
        using var context = new ChirpDBContext(builder.Options);
        context.initializeDB();
        IFollowRepository followRepository = new FollowRepository(context);
        var authorId = Guid.NewGuid().ToString();
        var followingId1 = Guid.NewGuid().ToString();
        var followingId2 = Guid.NewGuid().ToString();

        // Insert followings
        await followRepository.InsertNewFollowAsync(authorId, followingId1);
        await followRepository.InsertNewFollowAsync(authorId, followingId2);
        await context.SaveChangesAsync();

        // Act
        var followingCount = await followRepository.GetFollowingCountByAuthorIDAsync(authorId);

        // Assert
        Assert.Equal(2, followingCount);

        // Cleanup
        await _msSqlContainer.StopAsync();
    }


    // [Fact]
    // public async Task DeleteAuthorAsync()
    // {
    //     // Start the container
    //     await _msSqlContainer.StartAsync();

    //     // Arrange
    //     var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlServer(_connectionString);
    //     using var context = new ChirpDBContext(builder.Options);
    //     context.initializeDB(); //ensure all tables are created
    //     IAuthorRepository authorRepository = new AuthorRepository(context);
    //     var expectedName = "tan dang";

    //     await authorRepository.InsertAuthorAsync(expectedName, "");
    //     await context.SaveChangesAsync();

    //     // Act
    //     var authorDto = await authorRepository.GetAuthorByNameAsync(expectedName);
    //     await authorRepository.DeleteAuthorAsync(authorDto.AuthorId);
    //     await context.SaveChangesAsync();

    //     // Assert
    //     Assert.Null(await authorRepository.GetAuthorByNameAsync(expectedName));

    //     // Stop the container
    //     await _msSqlContainer.StopAsync();
    // }
}