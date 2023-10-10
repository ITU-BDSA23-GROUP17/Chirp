using Chirp.DTO;
using Chirp.Objects;
using Chirp.Repo.Library;

public interface ICheepService
{
    public IEnumerable<CheepDto> GetCheeps(int pageNr);
    public IEnumerable<CheepDto> GetCheepsFromAuthor(string author, int pageNr);
    public int getPagesHome(bool isAuthorPage, string? author);
}

public class CheepService : ICheepService
{
    // These woul normally be loaded from a database for example


    public IEnumerable<CheepDto> GetCheeps(int pageNr)
    {
        using (var context = new ChirpDBContext())
        {
            CheepRepository CR = new CheepRepository(context);

            var cheeps = from c in CR.GetCheeps()
                         select new CheepDto()
                         {
                             Id = c.CheepId,
                             Message = c.Text,
                             Author = c.Author.Name,
                             TimeStamp = c.TimeStamp
                         };

            return cheeps;
        }

    }

    public IEnumerable<CheepDto> GetCheepsFromAuthor(string author, int pageNr)
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
