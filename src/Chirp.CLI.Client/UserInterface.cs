using Newtonsoft.Json;
public static class UserInterface
{
    public static string? dateTime;

    async public static Task printCheeps(List<Cheep> cheeps, int? limit = null)
    {
        if (limit != null)
        {
            foreach (var cheep in cheeps)
            {
                dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToString("dd/MM/yyyy HH:mm:ss");
                Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
                limit--;
                if (limit == 0)
                {
                    break;
                }
            }

        }
        else
        {
            foreach (var cheep in cheeps)
            {
                dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToString("dd/MM/yyyy HH:mm:ss");
                Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
            }
        }
    }
}