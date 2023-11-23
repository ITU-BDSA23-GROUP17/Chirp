using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class CheepModel : PageModel
    {
        private IAuthorRepository _authorRepository;
        public CheepModel(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        [BindProperty]
        public string GetNewCheepText { get; set; }

        public async void OnPost()
        {

            Console.WriteLine(GetNewCheepText);
            var Claims = User.Claims;
            var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
            var author = await _authorRepository.GetAuthorByEmailAsync(email);
            await _authorRepository.SendCheepAsync(GetNewCheepText, new AuthorInfoDTO(author.AuthorId, author.Name, author.Email));
            // Redirect in the end
            Response.Redirect("/");
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
