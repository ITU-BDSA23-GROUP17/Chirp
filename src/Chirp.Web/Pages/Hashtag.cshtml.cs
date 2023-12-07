
using System.Drawing;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using static System.Web.HttpUtility;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Printing;

namespace Chirp.Web.Pages;

public class HashtagModel : PageModel
{
    //(We initialize with standard placeholder values to be overwritten later, to avoid
    //'Non-nullable property must contain a non-null value when exiting constructor.' warning'))
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; } = new List<CheepInfoDTO>();
    public int pageNr { get; set; } = 0;
    public int pages { get; set; } = 0;

    public AuthorDTO authorDTO { get; set; } = null;
    private string currentHashtagText;
    private AuthorDTO currentlyLoggedInUser;
    private List<String> cheepIds;
    public List<String> popularHashtags { get; set; } = null;


    [BindProperty]
    public IFormFile Upload { get; set; }


    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IFollowRepository _followRepository;
    private readonly IReactionRepository _reactionRepository;
    private readonly IHashtagRepository _hashtagRepository;

    public HashtagModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository,
    IReactionRepository reactionRepository, IHashtagRepository hashtagRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;
        _reactionRepository = reactionRepository;
        _hashtagRepository = hashtagRepository;
    }
    public async Task<IActionResult> OnGet(string hashtag)
    {

        // get user 

        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);


        //get popular hashtags
        popularHashtags = await _hashtagRepository.GetPopularHashtagsAsync();

        //get cheeps for current hashtag:

        currentHashtagText = hashtag;

        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));

        //To get cheeps we first get cheep ids from Hashtag repository, 
        //then the cheeps from the cheep repisotiry based on those ids
        //This all seems terribly inefficent and should probably be changed around
        //We need a many to many relationship between hashtags and cheeps, probably.

        cheepIds = _hashtagRepository.GetCheepIDsByHashtagText(currentHashtagText);
        if (cheepIds != null)
        {
            Cheeps = _cheepRepository.GetCheepsByCheepIds(cheepIds, pageNr);
        }
        else
        {
            throw new Exception("OH NO ?!");
        }

        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pages = _cheepRepository.getPagesFromCheepCount(Cheeps.Count());

        //We need to do some work to get the CheepInfo. First find Cheeps, then make CheepInfoDTOs.
        //We need the following ids in the else statement and below therefore it's here....
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();
        if (currentlyLoggedInUser != null)
        {
            List<string> followingIDs = await _followRepository.GetFollowingIDsByAuthorIDAsync(currentlyLoggedInUser.AuthorId);
            List<string> reactionCheepIds = await _reactionRepository.GetCheepIdsByAuthorId(currentlyLoggedInUser.AuthorId);
            CheepInfoList = new List<CheepInfoDTO>();
            //To get the CheepInfos we need to do some work...
            foreach (CheepDTO cheep in Cheeps)
            {
                Cheep = cheep,
                UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs),
                UserReactToCheep = IsUserReactionCheep(cheep.Id, reactionCheepIds),
                TotalReactions = await getTotalReactions(cheep.Id),
            };
            CheepInfoList.Add(cheepInfoDTO);
        }

        var viewModel = new ViewModel
        {
            pageNr = pageNr,
            pages = pages,
            CheepInfos = CheepInfoList,
            Cheeps = Cheeps
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

    public bool IsUserReactionCheep(string cheepId, List<string> reactionAuthorId)
    {
        {
            return reactionAuthorId.Contains(cheepId);
        }
    }

    public async Task<string> getTotalReactions(string cheepId)
    {
        var total = _reactionRepository.GetReactionByCheepId(cheepId);
        var totalLikes = total.Result.Count().ToString();
        if (totalLikes == "0")
        {
            return "0 Likes";
        }
        else if (totalLikes == "1")
        {
            return "1 Like";
        }
        else
        {
            return totalLikes + " Likes";
        }
    }


    public async Task<IActionResult> OnPostFollow(string authorName, string follow, string? unfollow)
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);
        var isUserFollowingAuthor = await _authorRepository.GetAuthorByIdAsync(authorName);

        Console.WriteLine(currentlyLoggedInUser);

        if (follow != null)
        {
            await _followRepository.InsertNewFollowAsync(currentlyLoggedInUser.AuthorId, authorName);
        }
        if (unfollow != null)
        {
            await _followRepository.RemoveFollowAsync(currentlyLoggedInUser.AuthorId, authorName);
        }


        return Redirect("/" + isUserFollowingAuthor.Name.Replace(" ", "%20"));
    }

    public async Task<IActionResult> OnPostReaction(string cheepId, string authorId, string reaction)
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;

        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

        var likeID = "fbd9ecd2-283b-48d2-b82a-544b232d6244";

        if (currentlyLoggedInUser == null)
        {
            Console.WriteLine("Can not react to cheep, user is not logged in");
        }
        bool hasReacted = await _reactionRepository.CheckIfAuthorReactedToCheep(cheepId, currentlyLoggedInUser.AuthorId);
        if (hasReacted)
        {
            Console.WriteLine("Removed like on " + cheepId);
            await _reactionRepository.RemoveReactionAsync(cheepId, currentlyLoggedInUser.AuthorId);
        }
        else
        {
            Console.WriteLine("Added like on " + cheepId);
            await _reactionRepository.InsertNewReactionAsync(cheepId, currentlyLoggedInUser.AuthorId, likeID);
        }


        //When using RedirectToPage() in / root and in public timline it will redirect to /Public, and /public is not a valid page. 
        if (HttpContext.Request.Path == "/Public")
        {
            return Redirect("/");
        }
        else
        {
            return RedirectToPage();
        }

    }

}
