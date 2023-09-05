using System.Text.RegularExpressions;

var path = "chirp_cli_db.csv";

if(args[0] == "read"){
    try
    {
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
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
} else if (args[0] == "cheep"){
    using (StreamWriter sw = File.AppendText(path))
    {
        if(args.Length == 1){
            Console.WriteLine("what do you want to chirp? because there is nothing :)");
        } else {
            string message = "";
            var user = Environment.UserName;
            DateTime dateTime = DateTime.Now;
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            long unixDateTime = dto.ToUnixTimeSeconds();
            foreach (var arg in args)
            {
                if(arg != args[0]){
                    message +=  arg == args[1] ? arg : $" {arg}";
                }
            }
            sw.WriteLine($"{user},{$"\"{message}\""},{unixDateTime}");
            Console.Write($"{user} @ {dateTime}: {message}");
        }
    }	
}
 




