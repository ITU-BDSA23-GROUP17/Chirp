using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
public static class UserInterface
{

    public static DateTime UnixTimeParser(long unixTime)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
    }

    async public static Task printCheeps(IEnumerable<Cheep> cheeps)
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

                //Link to library use to convert a string to JSON object
                //https://learn.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-using-the-dotnet-cli 
                // Now you can parse the JSON content or work with it as needed
                // For example, you can deserialize it into an object if you have a corresponding class
                // using a JSON library like Newtonsoft.Json (Json.NET)
                Cheep cheep = JsonConvert.DeserializeObject<Cheep>(jsonContent);
                DateTime dateTime = UnixTimeParser(cheep.Timestamp);
                
                Console.WriteLine($"{cheep.Author} @ {dateTime}: {cheep.Message} ");
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
}

