
using System.Drawing;
using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    //(We initialize with standard placeholder values to be overwritten later, to avoid
    //'Non-nullable property must contain a non-null value when exiting constructor.' warning'))
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public int pageNr { get; set; } = 0;
    public int pages { get; set; } = 0;

    public AuthorDTO authorDTO { get; set; } = null;


    [BindProperty]
    public IFormFile Upload { get; set; }


    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;

    }
 public IActionResult OnGet(string author)
{
    // Initialize your models here...
    var authorDTO = _authorRepository.GetAuthorByName(author);
    pages = _cheepRepository.getPagesUser(authorDTO.Name);
    pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
    Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);

    var viewModel = new ViewModel
    {
        Cheeps = Cheeps,
        pageNr = pageNr,
        pages = pages,      
    };
    
    ViewData["ViewModel"] = viewModel;

    return Page();
}

    public void OnPost()
    {
        // save image to database ? or maybe po
        Console.WriteLine(Upload.FileName);




    }

    public string getPageName()
    {
        return HttpContext.GetRouteValue("author").ToString();
    }




}
