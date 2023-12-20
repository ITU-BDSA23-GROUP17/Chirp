using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

public sealed class FollowRepositoryTest : IAsyncLifetime
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
}