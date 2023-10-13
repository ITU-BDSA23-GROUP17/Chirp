using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;

    public PublicModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }

    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }



    public ActionResult OnGet()
    {
        // pages = _service.getPagesHome(false, null);
        pages = _cheepRepository.getPages();
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheeps(pageNr);
        return Page();

    }
}
