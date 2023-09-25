using DocoptNet;


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
        UserInterface.printCheeps(int.Parse(arguments["<limit>"].ToString()));
    }
else if (arguments["cheep"].IsTrue)
{
    UserInterface.appendFile(arguments["<message>"].AsList.ToArray());
}


public record Cheep(string Author, string Message, long Timestamp)
{
    public string Author { get; init; } = Author;
    public string Message { get; init; } = Message;
    public long Timestamp { get; init; } = Timestamp;

};

