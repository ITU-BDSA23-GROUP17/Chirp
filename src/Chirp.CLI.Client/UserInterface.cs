public static class UserInterface
{

    public static DateTime UnixTimeParser(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
    }
    public static void printCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            DateTime dateTime = UnixTimeParser(long.Parse(cheep.Timestamp.ToString()));
            // DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(cheep.Timestamp.ToString())).UtcDateTime;
            Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
        }
    }
}

