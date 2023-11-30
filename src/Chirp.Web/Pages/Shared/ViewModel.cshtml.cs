using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
public class ViewModel
{
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; }
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();

    public int pageNr { get; set; }
    public int pages { get; set; }
    public AuthorDTO User { get; set; }

    public string idAccessToken { get; set; }
    public async Task<JObject> getGithub(string author)
    {
        var gitClaims = new GithubClaims();
        var claims = await gitClaims.GetGitHubClaimsAsync(author);
        return claims;
    }


}