using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text.RegularExpressions;
using SimpleDB;


if (args.Length == 0)
{
    Console.WriteLine("Welcome to chirp_cli");
    Console.WriteLine("To read chirps type: chirp_cli read");
    Console.WriteLine("To chirp type: chirp_cli cheep <message>");
}
else if (args[0] == "read")
{
    readFile();
}
else if (args[0] == "cheep")
{
    
};


// functions to read file
void readFile()
{
    var database = new CSVDatabase<Cheep>();
    foreach(var cheep in database.Read()){
            DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(cheep.Timestamp.ToString())).UtcDateTime;
            Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
    }
}

// function to append file
void appendFile(string[] args)
{
   /*  using (StreamWriter sw = File.AppendText(path))
    {
        if (args.Length == 1)
        {
            Console.WriteLine("what do you want to chirp? because there is nothing :)");
        }
        else
        {
            string message = "";
            var user = Environment.UserName;
            DateTime dateTime = DateTime.Now;
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            long unixDateTime = dto.ToUnixTimeSeconds();
            for (int i = 1; i < args.Length; i++)
            {
                message += i == 1 ? args[i] : $" {args[i]}";
            }
            sw.WriteLine($"{user},{$"\"{message}\""},{unixDateTime}");
            Console.Write($"{user} @ {dateTime}: {message}");
        }
    } */
};



public record Cheep(){
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }

};