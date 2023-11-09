
using System.Drawing;
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


    [BindProperty]
    public IFormFile Upload { get; set; }


    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;

    }
    public ActionResult OnGet(string author)
    {
        // get user
        _authorRepository.GetAuthorByName(author);

        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);

        pages = _cheepRepository.getPagesUser(author);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "0"));
        Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);


        return Page();
    }

    public void OnPost(){
        // save image to database ? or maybe po
        Console.WriteLine(Upload.FileName);




    }

    public string getPageName(){
        return  HttpContext.GetRouteValue("author").ToString();
    } 


}
