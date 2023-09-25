using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
public static class UserInterface
{

    public static DateTime UnixTimeParser(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
    }

    async public static Task printCheeps(int? limit = null)
    {
        // UserInterface.printCheeps(database.Read());
        // URL of the JSON file
        string jsonUrl = "https://bdsagroup17chirpremotedb.azurewebsites.net/cheeps";

        // Create an instance of HttpClient
        using HttpClient client = new HttpClient();

        try
        {
            // Send an HTTP GET request to the URL
            HttpResponseMessage response = await client.GetAsync(jsonUrl);

            // Check if the request was successful (status code 200)
            if (response.IsSuccessStatusCode)
            {
                // Read the JSON content as a string
                var jsonContent = await response.Content.ReadAsStringAsync();

                // Deserialize json object to list source https://stackoverflow.com/questions/34103498/how-do-i-deserialize-a-json-array-using-newtonsoft-json
                List<Cheep> cheeps = JsonConvert.DeserializeObject<List<Cheep>>(jsonContent);
                //Link to library use to convert a string to JSON object
                //https://learn.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-using-the-dotnet-cli 
                // Now you can parse the JSON content or work with it as needed
                // For example, you can deserialize it into an object if you have a corresponding class
                // using a JSON library like Newtonsoft.Json (Json.NET)
                foreach (var cheep in cheeps)
                {
                    DateTime dateTime = UnixTimeParser(cheep.Timestamp);
                    Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
                }
            }
            else
            {
                Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP request error: {ex.Message}");
        }
    }

    async public static Task appendFile(object[] args)
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
}


