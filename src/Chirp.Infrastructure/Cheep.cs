namespace Chirp.Infrastructure;

public class Cheep
{

    public string CheepId { get; set; }
    public Author Author { get; set; }

    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public string AuthorId { get; set; }

}