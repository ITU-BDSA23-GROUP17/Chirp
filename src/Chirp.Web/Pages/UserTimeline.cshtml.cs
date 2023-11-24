﻿
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
    private bool isOwnTimeline;
    public AuthorDTO authorDTO { get; set; } = null;
    private AuthorDTO currentlyLoggedInUser;


    [BindProperty]
    public IFormFile Upload { get; set; }


    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IFollowRepository _followRepository;
    private readonly IReactionRepository _reactionRepository;



    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository,
IReactionRepository reactionRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;
        _reactionRepository = reactionRepository;


    }
    public async Task<IActionResult> OnGet(string author)
    {
        List<CheepInfoDTO> CheepInfoList = new List<CheepInfoDTO>();

        // Initialize your models here...
        var authorDTO = await _authorRepository.GetAuthorByNameAsync(author);
        pages = _cheepRepository.getPagesUser(authorDTO.Name);
        pageNr = int.Parse(UrlDecode(Request.Query["page"].FirstOrDefault() ?? "1"));
        Cheeps = _cheepRepository.GetCheepsByAuthor(author, pageNr);

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



    public string getPageName()
    {
        return HttpContext.GetRouteValue("author").ToString();
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
        Console.WriteLine("cheepId: " + cheepId);
        Console.WriteLine("authorId: " + authorId);
        Console.WriteLine("reaction: " + reaction);
        Console.WriteLine("From UserTimeline.cshtml.cs");

        return Redirect("/");
    }

}
