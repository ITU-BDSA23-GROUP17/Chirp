
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    //(We initialize with standard placeholder values to be overwritten later, to avoid
    //'Non-nullable property must contain a non-null value when exiting constructor.' warning'))
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int pageNr { get; set; } = 0;
    public int pages { get; set; } = 0;


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
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "0"));
        var recievedCheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);
        if (recievedCheeps != null)
        {
            Cheeps = recievedCheeps;
        }
        else
        {
            Cheeps = new List<CheepDTO>();
        }

        return Page();
    }



}
