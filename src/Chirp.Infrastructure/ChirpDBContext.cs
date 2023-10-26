using Microsoft.EntityFrameworkCore;
namespace Chirp.Infrastructure;
public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }

    // source https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options) { }

    public void initializeDB()
    {
        Database.EnsureCreated(); //Ensures all tables are created!
        DbInitializer.SeedDatabase(this);
    }
}