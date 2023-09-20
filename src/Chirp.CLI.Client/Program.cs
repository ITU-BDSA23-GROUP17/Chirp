using SimpleDB;
using DocoptNet;

var database = CSVDatabase<Cheep>.Instance;

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
        UserInterface.printCheeps(database.Read());
    else
    {
        UserInterface.printCheeps(database.Read(int.Parse(arguments["<limit>"].ToString())));
    }
else if (arguments["cheep"].IsTrue)
{
    appendFile(arguments["<message>"].AsList.ToArray());
}
return 0;

/// <summary>
/// test
/// </summary> ///
void appendFile(object[] args)
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
    database.Store(newCheep);
    Console.Write($"added{user} @ {dateTime}: {$"\"{message}\""}");
};

public record Cheep(string Author, string Message, long Timestamp)
{
    public string Author { get; init; } = Author;
    public string Message { get; init; } = Message;
    public long Timestamp { get; init; } = Timestamp;

};

