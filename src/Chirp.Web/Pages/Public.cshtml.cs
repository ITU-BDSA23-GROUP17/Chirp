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
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var author = _authorRepository.GetAuthorByName(userName);

        if (User.Identity?.IsAuthenticated == true && (author == null || author.Name == null))
        {
            try
            {
                if (email != null)
                {
                    _authorRepository.InsertAuthor(userName, email);
                    _authorRepository.Save();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("author insert failed");
            }
        }

        // pages = _service.getPagesHome(false, null);
        pages = _cheepRepository.getPages();
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheeps(pageNr);

        var viewModel = new ViewModel
        {
            Cheeps = Cheeps,
            pageNr = pageNr,
            pages = pages,      
        };
    
        ViewData["ViewModel"] = viewModel;
        
        return Page();

    }
}
