using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;


    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;

    }






    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }



    public ActionResult OnGet()
    {
        // get user
        var userName = User.Identity?.Name;
        var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine(email);

        // if user does not exist create a new one
        if (User.Identity?.IsAuthenticated == true && _authorRepository.GetAuthorByName(userName) == null )
        {
                _authorRepository.InsertAuthor(userName,email);
                Console.WriteLine(email);
        }

        // pages = _service.getPagesHome(false, null);
        pages = _cheepRepository.getPages();
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheeps(pageNr);
        return Page();

    }
}
