using Chirp.Objects;
using Microsoft.EntityFrameworkCore;
public class CheepContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }
}