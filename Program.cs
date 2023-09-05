using System.Text.RegularExpressions;
var path = "chirp_cli_db.csv";

// main function
if (!File.Exists(path))
{
    Console.WriteLine("Does not exist");
}
else
{
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
        appendFile(args);
    }
}

// functions to read file
void readFile(){
        // Open the text file using a stream reader.
        using (var sr = new StreamReader(path))
        {
            Regex regex = new Regex("(?<abc>[A-Za-z]+),\"(?<message>[^\"]*)\",(?<date>[0-9]+)", RegexOptions.IgnoreCase);
            string data = sr.ReadLine();


            while ((data = sr.ReadLine()) != null)
            {
                MatchCollection matches = regex.Matches(data);
                foreach (Match match in matches)
                {
                    DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(match.Groups[3].ToString())).UtcDateTime;
                    Console.WriteLine($"{match.Groups[1]} @ {dateTime}: {match.Groups[2]} ");
                }
            }
        }
}

// function to append file
void appendFile(string[] args){
       using (StreamWriter sw = File.AppendText(path))
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
        }
}




