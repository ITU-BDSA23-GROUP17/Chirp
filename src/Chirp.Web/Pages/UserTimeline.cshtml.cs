
using System.Drawing;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication;
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

namespace Chirp.Web.Pages;

public class UserTimelineModel : BaseModel
{
    //(We initialize with standard placeholder values to be overwritten later, to avoid
    //'Non-nullable property must contain a non-null value when exiting constructor.' warning'))
    public IEnumerable<CheepDTO> Cheeps { get; set; } = new List<CheepDTO>();
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; } = new List<CheepInfoDTO>();
    public int pageNr { get; set; } = 0;
    public int pages { get; set; } = 0;
    private bool isOwnTimeline;

    public int followers;
    public int following;
    public string? authorImage;
    public AuthorDTO authorDTO { get; set; } = null;


    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository,
IReactionRepository reactionRepository) : base(cheepRepository, authorRepository, followRepository, reactionRepository)
    {
    }

    public async Task<IActionResult> OnGetAsync(string author)
    {
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();

        // Initialize your models here...
        var authorDTO = await _authorRepository.GetAuthorByNameAsync(author);

        if (authorDTO != null)
        {
            pages = _cheepRepository.getPagesUser(authorDTO.Name);
            pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
            Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);
            followers = await _followRepository.GetFollowerCountByAuthorIDAsync(authorDTO.AuthorId);
            following = await _followRepository.GetFollowingCountByAuthorIDAsync(authorDTO.AuthorId);
            authorImage = authorDTO.Image;
        }
        else
        {
            // Handle the case when authorDTO is null (author not found)
            // For example, you might return a 404 Not Found response.
            return NotFound();
        }
        // get user


        var email = User.Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);
        //compare the user of the page and the currently logged in user to decide which cheeps to show...

        isOwnTimeline = email != null && authorDTO != null && currentlyLoggedInUser.AuthorId == authorDTO.AuthorId;



        //source https://stackoverflow.com/questions/6514292/c-sharp-razor-url-parameter-from-view 
        // pages = _service.getPagesHome(true, author);
        pages = _cheepRepository.getPagesUser(authorDTO.Name);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        if (currentlyLoggedInUser != null)
        {
            //We need to do some work to get the CheepInfo. First find Cheeps, then make CheepInfoDTOs.
            //We need the following ids in the else statement and below therefore it's here....
            List<string> followingIDs = await _followRepository.GetFollowingIDsByAuthorIDAsync(currentlyLoggedInUser.AuthorId);
            List<string> reactionCheepIds = await _reactionRepository.GetCheepIdsByAuthorId(currentlyLoggedInUser.AuthorId);


            if (!isOwnTimeline)
            {
                Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);
            }
            else
            {
                List<string> authors = new List<string> { currentlyLoggedInUser.Name };

                List<AuthorDTO> follows = await _authorRepository.GetAuthorsByIdsAsync(followingIDs);

                reactionCheepIds = await _reactionRepository.GetCheepIdsByAuthorId(currentlyLoggedInUser.AuthorId);

                foreach (var followedAuthor in follows)
                {
                    authors.Add(followedAuthor.Name);
                }
                Cheeps = _cheepRepository.GetCheepsByAuthors(authors, pageNr);

            }

            //To get the CheepInfos we need to do some work...
            foreach (CheepDTO cheep in Cheeps)
            {
                CheepInfoDTO cheepInfoDTO = new CheepInfoDTO
                {
                    Cheep = cheep,
                    UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs),
                    UserReactToCheep = IsUserReactionCheep(cheep.Id, reactionCheepIds)
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


    public string getPageName()
    {
        return HttpContext.GetRouteValue("author").ToString();
    }



}