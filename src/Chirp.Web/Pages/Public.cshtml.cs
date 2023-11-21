using System.Diagnostics;
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
    private AuthorDTO currentlyLoggedInUser;


    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;
    }

    public IEnumerable<CheepDTO> Cheeps { get; set; }
    public IEnumerable<CheepInfoDTO> CheepInfos { get; set; }
    public int pageNr { get; set; }
    public int pages { get; set; }


    public ActionResult OnGet()
    {
        // get user

        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = _authorRepository.GetAuthorByEmail(email);
        var userName = currentlyLoggedInUser.Name;


        if (User.Identity?.IsAuthenticated == true && (currentlyLoggedInUser == null || currentlyLoggedInUser.Name == null))
        {
            try
            {
                if (email != null)
                {
                    _authorRepository.InsertAuthor(userName, email);
                    _authorRepository.Save();
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
        CheepInfos = Cheeps.Select(cheep => new CheepInfoDTO
        {
            Cheep = cheep,
            UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorName)
        }).ToList();

        return Page();
    }

    //Right now this method is on both public and user timeline. In general there is a lot of repeated code between the two. Seems silly...?
    public bool IsUserFollowingAuthor(string authorName)
    {
        var followingIDs = _followRepository.GetFollowingIDsByAuthorID(User.Identity.Name);
        {
            return followingIDs.Contains(authorName);
        }
    }

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
}
