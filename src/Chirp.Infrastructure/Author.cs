namespace Chirp.Infrastructure;

public class Author
{

    public string AuthorId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();


}
