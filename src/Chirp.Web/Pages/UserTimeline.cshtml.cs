
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }


    private readonly ICheepRepository _cheepRepository;

    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    public ActionResult OnGet(string author)
    {
        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pages = _cheepRepository.getPagesUser(author);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);

        return Page();
    }



}
