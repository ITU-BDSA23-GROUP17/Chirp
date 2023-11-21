
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
    public ActionResult OnGet(string author)
    {
        // get user
        var authorDTO = _authorRepository.GetAuthorByName(author);
        //get the user currently logged in:
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = _authorRepository.GetAuthorByEmail(email);
        //compare the user of the page and the currently logged in user to decide which cheeps to show...
        bool isOwnTimeline = currentlyLoggedInUser != null && authorDTO != null && currentlyLoggedInUser.AuthorId == authorDTO.AuthorId;



        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pages = _cheepRepository.getPagesUser(authorDTO.Name);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));

        if (!isOwnTimeline)
        {
            Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);
        }
        else
        {
            List<string> authors = new List<string> { currentlyLoggedInUser.Name };
            List<string> followingIDs = _followRepository.GetFollowingIDsByAuthorID(currentlyLoggedInUser.AuthorId);
            List<AuthorDTO> follows = _authorRepository.GetAuthorsByIds(followingIDs);
            foreach (var followedAuthor in follows)
            {
                authors.Add(followedAuthor.Name);
            }
            Cheeps = _cheepRepository.GetCheepsByAuthors(authors, pageNr);
        }

        CheepInfos = Cheeps.Select(cheep => new CheepInfoDTO
        {
            Cheep = cheep,
            UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorName)
        }).ToList();

        return Page();
    }

    public bool IsUserFollowingAuthor(string authorName)
    {
        var followingIDs = _followRepository.GetFollowingIDsByAuthorID(User.Identity.Name);
        {
            return followingIDs.Contains(authorName);
        }
    }

    //source https://www.learnrazorpages.com/razor-pages/handler-methods
    public void OnPost(string authorName, string follow, string unfollow)
    {
        if (follow != null)
        {
            _followRepository.InsertNewFollow(currentlyLoggedInUser.AuthorId, authorName);
        }
        if (unfollow != null)
        {
            _followRepository.RemoveFollow(currentlyLoggedInUser.AuthorId, authorName);
        }
    }

    public string getPageName()
    {
        return HttpContext.GetRouteValue("author").ToString();
    }




}
