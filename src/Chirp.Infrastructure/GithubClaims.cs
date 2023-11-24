using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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


    public async Task<string> GetGitHubClaimsUserImageAsync(string username)
    {
        string ImageUrl = null;
        JObject claims = await GetGitHubClaimsAsync(username);


        if (claims.Value<string>("message") != "not found")
        {
            ImageUrl = claims.Value<string>("avatar_url");
        }
        else
        {
            ImageUrl = "https://assets.phillips.com/image/upload/t_Website_LotDetailMainImage/v1/auctions/NY000208/52_001.png";
        }
        return ImageUrl;
    }
}