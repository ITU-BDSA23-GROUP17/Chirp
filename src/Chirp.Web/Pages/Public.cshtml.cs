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
        //To get the CheepInfos we need to do some work...
        List<string> followingIDs = _followRepository.GetFollowingIDsByAuthorID(currentlyLoggedInUser.AuthorId);
        Console.Write("THE LENGTH OF THE FOLLOWINGIDS LIST IS ");
        Console.WriteLine((followingIDs).Count);
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();
        foreach (CheepDTO cheep in Cheeps)
        {
            CheepInfoDTO cheepInfoDTO = new CheepInfoDTO { Cheep = cheep, UserIsFollowingAuthor = IsUserFollowingAuthor(cheep.AuthorName, followingIDs) };
            CheepInfoList.Add(cheepInfoDTO);
        }

        CheepInfos = CheepInfoList;

        if (Cheeps == null)
        {
            throw new Exception("CHEEPS ARE NULL NOW! ");
        }
        return Page();
    }

    //Right now this method is on both public and user timeline. In general there is a lot of repeated code between the two. Seems silly...?
    public bool IsUserFollowingAuthor(string authorName, List<string> followingIDs)
    {
        {
            bool isFollowing = followingIDs.Contains(authorName);
            if (isFollowing)
            {
                Console.WriteLine("THE USER IS ALREADY FOLLOWING THIS PERSON!");
            }
            else
            {
                Console.WriteLine("NOT FOLLOWING THIS PERSON!");
            }
            return isFollowing;
        }
    }

    public void OnPost(string authorName, string follow, string unfollow)
    {
        //We do this in OnGet (retrieve current user). Surely there is a way to save that and reuse it here? but we can't just save it as a field in this class. That doesn't work....
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = _authorRepository.GetAuthorByEmail(email);

        if (follow != null)
        {
            _followRepository.InsertNewFollow(currentlyLoggedInUser.AuthorId, authorName);
        }
        if (unfollow != null)
        {
            _followRepository.RemoveFollow(currentlyLoggedInUser.AuthorId, authorName);
        }
        // Redirect in the end
        Response.Redirect("/");
    }
}
