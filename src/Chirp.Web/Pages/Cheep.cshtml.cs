using System.Text.RegularExpressions;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class CheepModel : PageModel
    {
        private IAuthorRepository _authorRepository;
        private IHashtagRepository _hashtagRepository;
        private IHashtagTextRepository _hashtagTextRepository;

        // suppress warnings
        #pragma warning disable CS8618

        public CheepModel(IAuthorRepository authorRepository, IHashtagRepository hashtagRepository, IHashtagTextRepository hashtagTextRepository)
        {
            _authorRepository = authorRepository;
            _hashtagRepository = hashtagRepository;
            _hashtagTextRepository = hashtagTextRepository;
        }

        [BindProperty]
        public string GetNewCheepText { get; set; }


        public async Task<IActionResult> OnPost()
        {

            var Claims = User.Claims;
            var name = Claims.FirstOrDefault(c => c.Type == "name")?.Value;
            AuthorDTO? author = null;
            CheepDTO? cheep = null;
            if (name != null)
            {
                author = await _authorRepository.GetAuthorByNameAsync(name);
            }

            if (author != null)
            {
                cheep = await _authorRepository.SendCheepAsync(GetNewCheepText, new AuthorInfoDTO(author.AuthorId, author.Name, author.Email));
            }

            //use regex to recognize hashtags from the cheep text
            Regex regex = new Regex(@"#\w+");
            MatchCollection matches = regex.Matches(GetNewCheepText);
            //cycle through the matches, and add them to the hashtag repository with the cheep.
            if (cheep != null && cheep.Id != null)
            {

                foreach (Match match in matches)
                {
                    string hashtagText = match.ToString().TrimStart('#');
                    await _hashtagRepository.InsertNewHashtagCheepPairingAsync(hashtagText, cheep.Id);
                    await _hashtagTextRepository.AddHashtag(hashtagText);
                }
            }

            // Redirect in the end
            return Redirect("/");
        }
        public void OnGet()
        {
            if (User != null && User.Identity != null && !User.Identity.IsAuthenticated)
            {

                Response.Redirect("/MicrosoftIdentity/Account/SignIn");

            }
        }
    }
}
