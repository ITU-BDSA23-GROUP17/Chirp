using System.Diagnostics;
using System.Drawing;
using System.Security.Claims;
using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Web.HttpUtility;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IFollowRepository _followRepository;
    private readonly IReactionRepository _reactionRepository;
    private AuthorDTO currentlyLoggedInUser;


    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository,
IReactionRepository reactionRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;
        _reactionRepository = reactionRepository;
    }

    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }

    public async Task<ActionResult> OnGetAsync()
    {
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();

        // get user

        var Claims = User.Claims;

        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        var username = Claims.FirstOrDefault(c => c.Type == "name")?.Value;

        if (User.Identity?.IsAuthenticated == true && (currentlyLoggedInUser == null || currentlyLoggedInUser.Name == null))
        {
            try
            {
                if (email != null)
                {
                    await _authorRepository.InsertAuthorAsync(username, email);
                    await _authorRepository.SaveAsync();
                    currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("author insert failed");
            }
        }




        // pages = _service.getPagesHome(false, null);
        pages = _cheepRepository.getPages();
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheeps(pageNr);
        if (currentlyLoggedInUser != null)
        {
            currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

            //To get the CheepInfos we need to do some work...
            List<string> followingIDs = await _followRepository.GetFollowingIDsByAuthorIDAsync(currentlyLoggedInUser.AuthorId);

            foreach (CheepDTO cheep in Cheeps)
            {
                CheepInfoDTO cheepInfoDTO = new CheepInfoDTO { Cheep = cheep, UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorId, followingIDs) };
                CheepInfoList.Add(cheepInfoDTO);
            }

        }

        var viewModel = new ViewModel
        {
            pageNr = pageNr,
            pages = pages,
            CheepInfos = CheepInfoList,
            Cheeps = Cheeps,
            User = currentlyLoggedInUser
        };

        ViewData["ViewModel"] = viewModel;


        return Page();
    }

    //Right now this method is on both public and user timeline. In general there is a lot of repeated code between the two. Seems silly...?
    public bool IsUserFollowingAuthor(string authorID, List<string> followingIDs)
    {
        {
            return followingIDs.Contains(authorID);
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
        Console.WriteLine("reaction: " + reaction);

        return null;
    }

}
