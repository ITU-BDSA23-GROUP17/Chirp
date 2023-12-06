using Microsoft.EntityFrameworkCore;
namespace Chirp.Infrastructure;
public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<ReactionType> ReactionTypes { get; set; }
    public DbSet<Follow> Followings { get; set; }
    public DbSet<Hashtag> Hashtags { get; set; }

    // source https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reaction>()
            .HasKey(r => new { r.CheepId, r.AuthorId }); // Composite primary key
        modelBuilder.Entity<Follow>()
           .HasKey(f => new { f.FollowerId, f.FollowingId }); // Composite primary key
        modelBuilder.Entity<Hashtag>()
           .HasKey(h => new { h.CheepID, h.HashtagText }); // Composite primary key



        // Other configurations if needed

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("your_connection_string", b => b.MigrationsAssembly("Chirp.Web"));
        }
    }
    public void initializeDB()
    {
        // Database.Migrate();
        Database.EnsureCreated(); //Ensures all tables are created!
        DbInitializer.SeedDatabase(this);
    }

}