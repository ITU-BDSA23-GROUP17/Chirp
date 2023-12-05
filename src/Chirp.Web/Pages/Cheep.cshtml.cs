using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class CheepModel : PageModel
    {
        private IAuthorRepository _authorRepository;
        private IHashtagRepository _hashtagRepository;
        public CheepModel(IAuthorRepository authorRepository, IHashtagRepository hashtagRepository)
        {
            _authorRepository = authorRepository;
            _hashtagRepository = hashtagRepository;
        }

        [BindProperty]
        public string GetNewCheepText { get; set; }


        public async Task<IActionResult> OnPost()
        {

            var Claims = User.Claims;
            var name = Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            var author = await _authorRepository.GetAuthorByNameAsync(name);

            var cheep = await _authorRepository.SendCheepAsync(GetNewCheepText, new AuthorInfoDTO(author.AuthorId, author.Name, author.Email));

            //use regex to recognize hashtags from the cheep text
            Regex regex = new Regex(@"#\w+");
            MatchCollection matches = regex.Matches(GetNewCheepText);
            //cycle through the matches, and add them to the hashtag repository with the cheep.
            foreach (Match match in matches)
            {
                await _hashtagRepository.InsertNewHashtagCheepPairingAsync(match.ToString(), cheep.Id);
            }

            // Redirect in the end
            return Redirect("/");
        }
        public void OnGet()
        {
            if (!User.Identity.IsAuthenticated)
            {

                Response.Redirect("/MicrosoftIdentity/Account/SignIn");

            }
        }
    }
}
