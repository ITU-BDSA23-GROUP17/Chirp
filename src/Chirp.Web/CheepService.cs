using Chirp.Core;
using Chirp.Infrastructure;
public interface ICheepService
{
    public IEnumerable<CheepDTO> GetCheeps(int pageNr);
    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int pageNr);
    public int getPagesHome(bool isAuthorPage, string? author);
}

public class CheepService : ICheepService
{

    public IEnumerable<CheepDTO> GetCheeps(int pageNr)
    {
        Console.WriteLine("hej");
        return null;
    }

    public IEnumerable<CheepDTO> GetCheepsFromAuthor(string author, int pageNr)
    {
        Console.WriteLine("hej");
        return null;
    }

    public static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    public int getPagesHome(bool isAuthorPage, string? author)
    {
        Console.WriteLine("hej");
        return 0;
    }
}
