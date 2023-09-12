public static class UserInterface
{
    public static void printCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(cheep.Timestamp.ToString())).UtcDateTime;
            Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
        }
    }
}
