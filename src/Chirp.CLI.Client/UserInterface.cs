using Newtonsoft.Json;
public static class UserInterface
{
    private static HttpClient? client;
    public static string? dateTime;

    async public static Task printCheeps(int? limit = null)
    {
        // UserInterface.printCheeps(database.Read());
        // URL of the JSON file
        string jsonUrl = "https://bdsagroup17chirpremotedb.azurewebsites.net/cheeps";

        // Create an instance of HttpClient
        client = new HttpClient();

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
                //Link to library use to convert a string to JSON object
                //https://learn.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-using-the-dotnet-cli 
                // Now you can parse the JSON content or work with it as needed
                // For example, you can deserialize it into an object if you have a corresponding class
                // using a JSON library like Newtonsoft.Json (Json.NET)
                List<Cheep> cheeps = JsonConvert.DeserializeObject<List<Cheep>>(jsonContent);

                foreach (var cheep in cheeps)
                {
                    dateTime = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToString("dd/MM/yyyy HH:mm:ss");
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
}