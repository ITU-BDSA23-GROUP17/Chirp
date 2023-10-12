using System.Web;
using Chirp.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public IEnumerable<CheepDto> Cheeps { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }


    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "0"));
        Cheeps = _service.GetCheepsFromAuthor(author, pageNr);
        return Page();
    }



}
