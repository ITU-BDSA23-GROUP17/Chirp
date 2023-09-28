using DocoptNet;
using System.Net.Http.Json;

async static Task appendFile(object[] args)
{

    // source https://stackoverflow.com/questions/68719641/send-json-data-in-http-post-request-c-sharp

    string jsonUrl = "https://bdsagroup17chirpremotedb.azurewebsites.net/cheep";

    try
    {
        string message = "";
        var user = Environment.UserName;
        DateTime dateTime = DateTime.Now;
        DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
        long unixDateTime = dto.ToUnixTimeSeconds();
        for (int i = 0; i < args.Length; i++)
        {
            message += i == 0 ? args[i] : $" {args[i]}";
        }
        Cheep newCheep = new Cheep(user, message, unixDateTime);

        // Create an instance of HttpClient
        using HttpClient client = new HttpClient();

        var response = client.PostAsJsonAsync(jsonUrl, newCheep).Result;


        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Added {user} @ {dateTime}: \"{message}\"");
        }
        else
        {
            Console.WriteLine($"Failed to add cheep. Status code: {response.StatusCode}");
        }
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"HTTP request error: {ex.Message}");
    }
}


const string help = @"chirp.

    Usage:
    chirp read <limit> | read
    chirp cheep <message>...
    chirp ---h | ---help


    Options:
    ---h ---help     Show this screen.
";


var arguments = new Docopt().Apply(help, args, version: "Chirp 1.0", exit: true)!;

if (arguments["---help"].IsTrue | arguments["---h"].IsTrue)
    Console.WriteLine(help);
else if (arguments["read"].IsTrue)
    if (arguments["<limit>"].ToString().Equals(""))
    {
        await UserInterface.printCheeps();
    }
    else
    {
        await UserInterface.printCheeps(int.Parse(arguments["<limit>"].ToString()));
    }
else if (arguments["cheep"].IsTrue)
{
    appendFile(arguments["<message>"].AsList.ToArray());
}


public record Cheep(string Author, string Message, long Timestamp)
{
    public string Author { get; init; } = Author;
    public string Message { get; init; } = Message;
    public long Timestamp { get; init; } = Timestamp;

};

