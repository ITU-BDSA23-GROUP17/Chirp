using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class CheepModel : PageModel
    {
        private ICheepRepository _cheepRepository;
        private IAuthorRepository _authorRepository;
        public CheepModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
        {
            _cheepRepository = cheepRepository;
            _authorRepository = authorRepository;
        }

        [BindProperty]
        public string GetNewCheepText { get; set; }

        public void OnPost()
        {
            var currentUser = _authorRepository.GetAuthorByName(User.Identity.Name);

            //We create a cheep
            var cheepDto = new CheepDTO(
                Id: "",
                Message: GetNewCheepText,
                TimeStamp: DateTime.Now,
                AuthorName: User.Identity.Name,
                AuthorId: currentUser.AuthorId
                );

            _cheepRepository.InsertCheep(cheepDto);

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
