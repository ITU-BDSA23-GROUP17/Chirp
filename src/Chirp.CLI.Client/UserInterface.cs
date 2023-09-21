using System;
using System.Net.Http;
using System.Threading.Tasks;
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
                string jsonContent = await response.Content.ReadAsStringAsync();

                // Now you can parse the JSON content or work with it as needed
                // For example, you can deserialize it into an object if you have a corresponding class
                // using a JSON library like Newtonsoft.Json (Json.NET)
                // For simplicity, let's just print the JSON content in this example
                Console.WriteLine(jsonContent);
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

