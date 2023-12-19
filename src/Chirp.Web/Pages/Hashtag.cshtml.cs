using Chirp.Core;
using Microsoft.AspNetCore.Mvc;
using static System.Web.HttpUtility;

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

    public AuthorDTO authorDTO { get; set; }
    private string currentHashtagText;
    private new AuthorDTO? currentlyLoggedInUser;
    private List<string> cheepIds;
    public List<string> uniqueHashtagTexts { get; set; }
    public List<string> popularHashtags { get; set; }

    private readonly IHashtagRepository _hashtagRepository;
    private readonly IHashtagTextRepository _hashtagTextRepository;

    // suppress warnings
    #pragma warning disable CS8618

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

        if (email != null)
        {
            currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);
        }


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



        if (currentlyLoggedInUser != null && email != null)
        {
            currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

            List<string>? followingIDs = null;
            List<string>? reactionCheepIds = null;

            if (currentlyLoggedInUser != null)
            {
                //To get the CheepInfos we need to do some work...
                followingIDs = await _followRepository.GetFollowingIDsByAuthorIDAsync(currentlyLoggedInUser.AuthorId);
                reactionCheepIds = await _reactionRepository.GetCheepIdsByAuthorId(currentlyLoggedInUser.AuthorId);

            }


            if (followingIDs != null && reactionCheepIds != null)
            {
                foreach (CheepDTO cheep in Cheeps)
                {
                    CheepInfoDTO cheepInfoDTO = new CheepInfoDTO
                    {
                        Cheep = cheep,
                        UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs),
                        UserReactToCheep = IsUserReactionCheep(cheep.Id, reactionCheepIds),
                        TotalReactions = getTotalReactions(cheep.Id),

                    };
                    CheepInfoList.Add(cheepInfoDTO);
                }
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

    public string getTotalReactions(string cheepId)
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