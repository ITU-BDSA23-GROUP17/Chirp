using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages
{
    public class CheepModel : PageModel
    {
        private ICheepRepository _cheepRepository;
        public CheepModel(ICheepRepository cheepRepository)
        {
            _cheepRepository = cheepRepository;
        }

        [BindProperty]
        public string GetNewCheepText { get; set; }

        public void OnPost()
        {
            _cheepRepository.SendCheep(GetNewCheepText);

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
