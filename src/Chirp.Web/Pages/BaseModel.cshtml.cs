using Chirp.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class BaseModel : PageModel
{
    protected ICheepRepository _cheepRepository;
    protected IAuthorRepository _authorRepository;
    protected IFollowRepository _followRepository;
    protected IReactionRepository _reactionRepository;
    protected AuthorDTO currentlyLoggedInUser;

    public BaseModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository, IFollowRepository followRepository, IReactionRepository reactionRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
        _followRepository = followRepository;
        _reactionRepository = reactionRepository;
    }

    public bool IsUserFollowingAuthor(string authorID, List<string> followingIDs)
    {
        return followingIDs.Contains(authorID);
    }

    public bool IsUserReactionCheep(string cheepId, List<string> reactionAuthorId)
    {
        return reactionAuthorId.Contains(cheepId);
    }

    public async Task<string> getStatus()
    {
        string? viewedUser = HttpContext?.GetRouteValue("author")?.ToString();
        var StatusAuthorDTO = await _authorRepository.GetAuthorByNameAsync(viewedUser);
        var Status = StatusAuthorDTO?.Status;
        Console.WriteLine("Received user: " + viewedUser);
        Console.WriteLine("Received status: " + Status);
        return Status;
    }

    public async Task<string> getStatusPublic(string name)
    {
        var StatusAuthorDTO = await _authorRepository.GetAuthorByNameAsync(name);
        var Status = StatusAuthorDTO?.Status;
        Console.WriteLine("Received user: " + name);
        Console.WriteLine("Received status: " + Status);
        return Status;
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

        await _authorRepository.UpdateAuthorStatusAsync(currentlyLoggedInUser?.Email);

        return Redirect("/");
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
            await _reactionRepository.InsertNewReactionAsync(cheepId, currentlyLoggedInUser.AuthorId);
        }

        Console.WriteLine(HttpContext.Request.Path);

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

    public async Task<IActionResult> OnPostStatus()
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

        await _authorRepository.UpdateAuthorStatusAsync(currentlyLoggedInUser?.Email);

        return Redirect("/");
    }

    public async Task<IActionResult> OnPostStatusUnavailable()
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

        await _authorRepository.UpdateAuthorStatusUnavailable(currentlyLoggedInUser?.Email);

        return Redirect("/");
    }

    public async Task<IActionResult> OnPostStatusOnline()
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

        await _authorRepository.UpdateAuthorStatusOnline(currentlyLoggedInUser?.Email);

        return Redirect("/");
    }

    public async Task<IActionResult> OnPostStatusOffline()
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

        await _authorRepository.UpdateAuthorStatusOffline(currentlyLoggedInUser?.Email);

        return Redirect("/");
    }

    public async Task<IActionResult> OnPostSetStatusOfflineAsync()
    {
        var Claims = User.Claims;
        var email = Claims.FirstOrDefault(c => c.Type == "emails")?.Value;
        currentlyLoggedInUser = await _authorRepository.GetAuthorByEmailAsync(email);

        await _authorRepository.UpdateAuthorStatusAsync(currentlyLoggedInUser?.Email);

        return SignOut(new AuthenticationProperties { RedirectUri = "/MicrosoftIdentity/Account/SignedOut" }, "Cookies");
    }
}