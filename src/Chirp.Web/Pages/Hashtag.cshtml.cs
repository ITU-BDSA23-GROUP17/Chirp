
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

public class HashtagModel : BaseModel
{
    //(We initialize with standard placeholder values to be overwritten later, to avoid
    //'Non-nullable property must contain a non-null value when exiting constructor.' warning'))
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; } = new List<CheepInfoDTO>();
    List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();

    public int pageNr { get; set; } = 0;
    public int pages { get; set; } = 0;

    public AuthorDTO authorDTO { get; set; } = null;
    private string currentHashtagText;
    private AuthorDTO currentlyLoggedInUser;
    private List<String> cheepIds;
    public List<String> uniqueHashtagTexts { get; set; } = null;
    public List<String> popularHashtags { get; set; } = null;

    private readonly IHashtagRepository _hashtagRepository;
    private readonly IHashtagTextRepository _hashtagTextRepository;

    public HashtagModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository,
    IReactionRepository reactionRepository, IHashtagRepository hashtagRepository, IHashtagTextRepository hashtagTextRepository) : base(cheepRepository, authorRepository, followRepository, reactionRepository)
    {
        _hashtagRepository = hashtagRepository;
        _hashtagTextRepository = hashtagTextRepository;
    }

    public async Task<IActionResult> OnGet(string hashtag)
    {

        // get user 

        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);


        //get popular hashtags
        uniqueHashtagTexts = await _hashtagTextRepository.GetUniqueHashtagTextsAsync();
        popularHashtags = _hashtagRepository.GetPopularHashtags(uniqueHashtagTexts);

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
            throw new Exception("Failed to retrieve cheep id from hashtag text");
        }

        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pages = _cheepRepository.getPagesFromCheepCount(Cheeps.Count());



        if (currentlyLoggedInUser != null)
        {
            currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

            //To get the CheepInfos we need to do some work...
            List<string> followingIDs = await _followRepository.GetFollowingIDsByAuthorIDAsync(currentlyLoggedInUser.AuthorId);
            List<string> reactionCheepIds = await _reactionRepository.GetCheepIdsByAuthorId(currentlyLoggedInUser.AuthorId);


            foreach (CheepDTO cheep in Cheeps)
            {
                CheepInfoDTO cheepInfoDTO = new CheepInfoDTO
                {
                    Cheep = cheep,
                    UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs),
                    UserReactToCheep = IsUserReactionCheep(cheep.Id, reactionCheepIds),
                    TotalReactions = await getTotalReactions(cheep.Id),

                };
                CheepInfoList.Add(cheepInfoDTO);
            }

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
}