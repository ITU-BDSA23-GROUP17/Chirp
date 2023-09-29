public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example

    public List<CheepViewModel> GetCheeps()
    {
        // DBFacade DB = new DBFacade();
        return DBFacade.DatabaseQuery(@"SELECT username as Author, text as Message, pub_date as Timestamp FROM message JOIN user ON author_id = user_id");
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return DBFacade.DatabaseQuery($"SELECT username as Author, text as Message, pub_date as Timestamp FROM message JOIN user ON author_id = user_id WHERE \"{author}\" = Author");
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

}
