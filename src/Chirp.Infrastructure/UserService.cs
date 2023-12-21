using Chirp.Core;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Newtonsoft.Json;

public class UserService : IUserService
{

    public GraphServiceClient? graphClient;
    public UserService(string? ClientID, string? TenantId, string? ClientSecret)
    {
        try
        {
            // Initialize the client credential auth provider
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                .Create(ClientID)
                .WithTenantId(TenantId)
                .WithClientSecret(ClientSecret)
                .Build();
            ClientCredentialProvider authProvider = new ClientCredentialProvider(confidentialClientApplication);

            // Set up the Microsoft Graph service client with client credentials
            graphClient = new GraphServiceClient(authProvider);
        }
        catch
        {
            Console.WriteLine("Could not create graph client");
        }
    }

    public async Task DeleteUserById(string? userId)
    {
        Console.WriteLine($"Looking for user with object ID '{userId}'...");

        try
        {
            // Delete user by object ID
            await graphClient!.Users[userId]
                .Request()
                .DeleteAsync();

            Console.WriteLine($"User with object ID '{userId}' successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            Console.WriteLine("This means that the user has not given the correct permissions to the application:");
            Console.WriteLine("Learn more: https://learn.microsoft.com/en-us/azure/active-directory-b2c/add-web-api-application?tabs=app-reg-ga");
        }
    }

    public async Task ListUsers()
    {
        Console.WriteLine("Getting list of users...");

        try
        {
            // Get all users (one page)
            if (graphClient != null)
            {
                var result = await graphClient.Users
                    .Request()
                    .Select(e => new
                    {
                        e.DisplayName,
                        e.Id,
                        e.Identities
                    })
                    .GetAsync();

                foreach (var user in result.CurrentPage)
                {
                    Console.WriteLine(JsonConvert.SerializeObject(user));
                }
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ResetColor();
            Console.WriteLine("This means that the user has not given the correct permissions to the application:");
            Console.WriteLine("Learn more: https://learn.microsoft.com/en-us/azure/active-directory-b2c/add-web-api-application?tabs=app-reg-ga");
        }




    }
}