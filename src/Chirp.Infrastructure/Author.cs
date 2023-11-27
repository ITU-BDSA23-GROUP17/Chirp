namespace Chirp.Infrastructure;

public class Author
{

    public required string AuthorId { get; set; }
    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Status { get; set; }

    public List<Cheep> Cheeps { get; set; } = new();


}
