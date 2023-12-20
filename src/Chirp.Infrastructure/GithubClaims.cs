using Newtonsoft.Json.Linq;

/*
    The warning is suppressed, as we handle the possible
    exception that might be raised, if token conversion fails

*/

#pragma warning disable CS8600

public class GithubClaims
{
    private static readonly HttpClient client = new HttpClient();

    public async Task<JObject> GetGitHubClaimsAsync(string username)
    {
        Console.WriteLine("getting user claim");

        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; GrandCircus/1.0)");

        var stringTask = client.GetStringAsync($"https://api.github.com/users/{username}");


        var msg = await stringTask;

        return JObject.Parse(msg);
    }


    public async Task<string?> GetGitHubClaimsUserImageAsync(string username)
    {
        string ImageUrl = "https://assets.phillips.com/image/upload/t_Website_LotDetailMainImage/v1/auctions/NY000208/52_001.png";

        try
        {

            JObject claims = await GetGitHubClaimsAsync(username);

            ImageUrl = claims.Value<string>("avatar_url");

        }
        catch (System.Exception)
        {
            Console.WriteLine("user does not have a git profile");
        }

        if(ImageUrl != null){
            return ImageUrl;
        } else {
            return null;
        }


    }
}