
using System.Drawing;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Microsoft.VisualBasic;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    //(We initialize with standard placeholder values to be overwritten later, to avoid
    //'Non-nullable property must contain a non-null value when exiting constructor.' warning'))
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; } = new List<CheepInfoDTO>();
    public int pageNr { get; set; } = 0;
    public int pages { get; set; } = 0;

    public AuthorDTO authorDTO { get; set; } = null;
    private AuthorDTO currentlyLoggedInUser;


    [BindProperty]
    public IFormFile Upload { get; set; }


    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IFollowRepository _followRepository;


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;

    }
    public async Task<IActionResult> OnGet(string author)
    {
        // Initialize your models here...
        var authorDTO = await _authorRepository.GetAuthorByNameAsync(author);
        pages = _cheepRepository.getPagesUser(authorDTO.Name);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);

        // get user
        //get the user currently logged in:
        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        //compare the user of the page and the currently logged in user to decide which cheeps to show...
        bool isOwnTimeline = email != null && authorDTO != null && currentlyLoggedInUser.AuthorId == authorDTO.AuthorId;



        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pages = _cheepRepository.getPagesUser(authorDTO.Name);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));

        //We need to do some work to get the CheepInfo. First find Cheeps, then make CheepInfoDTOs.
        //We need the following ids in the else statement and below therefore it's here....
        List<string> followingIDs = _followRepository.GetFollowingIDsByAuthorID(currentlyLoggedInUser.AuthorId);
        if (!isOwnTimeline)
        {
            Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);
        }
        else
        {
            List<string> authors = new List<string> { currentlyLoggedInUser.Name };

            List<AuthorDTO> follows = await _authorRepository.GetAuthorsByIdsAsync(followingIDs);
            foreach (var followedAuthor in follows)
            {
                authors.Add(followedAuthor.Name);
            }
            Cheeps = _cheepRepository.GetCheepsByAuthors(authors, pageNr);
        }
        //To get the CheepInfos we need to do some work...
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();
        foreach (CheepDTO cheep in Cheeps)
        {
            CheepInfoDTO cheepInfoDTO = new CheepInfoDTO { Cheep = cheep, UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs) };
            CheepInfoList.Add(cheepInfoDTO);
        }

        CheepInfos = CheepInfoList;

        var viewModel = new ViewModel
        {
            pageNr = pageNr,
            pages = pages,
        };


        ViewData["ViewModel"] = viewModel;

        return Page();
    }

    public bool IsUserFollowingAuthor(string authorID, List<string> followingIDs)
    {
        {
            return followingIDs.Contains(authorID);
        }
    }



    public string getPageName()
    {
        return HttpContext.GetRouteValue("author").ToString();
    }




}
