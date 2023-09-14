using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;
using SimpleDB;
using DocoptNet;

var database = new CSVDatabase<Cheep>();

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
    Cheep newCheep = new Cheep();
    newCheep.Message = message;
    newCheep.Author = user;
    newCheep.Timestamp = unixDateTime;
    database.Store(newCheep);
    Console.Write($"{user} @ {dateTime}: {$"\"{message}\""}");
};

static DateTime UnixTimeParser(long unixTime)
{
    return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
};
public record Cheep()
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }

};

