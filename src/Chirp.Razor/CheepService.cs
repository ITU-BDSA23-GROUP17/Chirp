public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int pageNr);
    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNr);
    public int getPagesHome(bool isAuthorPage, string? author);
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example

    DBFacade DB = new DBFacade();
    public List<CheepViewModel> GetCheeps(int pageNr)
    {
        // calculate offset formula 
        var offset = pageNr * 32;
        // DBFacade DB = new DBFacade();
        return DB.DatabaseQuery($@"SELECT username as Author, text as Message, pub_date as Timestamp FROM message JOIN user ON author_id = user_id ORDER BY pub_date DESC LIMIT 32 OFFSET {offset}");
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author, int pageNr)
    {
        // calculate offset formula 
        var offset = pageNr * 32;
        // filter by the provided author name
        return DB.DatabaseQuery($"SELECT username as Author, text as Message, pub_date as Timestamp FROM message JOIN user ON author_id = user_id WHERE \"{author}\" = Author ORDER BY pub_date DESC LIMIT 32 OFFSET {offset}");
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
        if (isAuthorPage)
        {
            return DB.CountQuery($"SELECT CAST(COUNT(*) as float) / 32 AS CountDividedBy FROM message JOIN user ON author_id = user_id WHERE \"{author}\" = username ");
        }
        else
        {
            return DB.CountQuery(@"SELECT  CAST(COUNT(*) as float) / 32 AS CountDividedBy FROM message");
        }

    }
}
