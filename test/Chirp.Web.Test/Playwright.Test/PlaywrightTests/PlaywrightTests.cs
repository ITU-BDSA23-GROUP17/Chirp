using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class PlaywrightTests : PageTest
{
    [Test]
    public async Task CheckElementPresence()
    {

        // get the website 
        await Page.GotoAsync("https://bdsagroup17chirprazor.azurewebsites.net/");
        Console.WriteLine("Website is live");
        // Expect a title to have title Chirp!.
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));

        // Check that the "cheep" element is present
        var cheepElement = await Page.QuerySelectorAsync(".text-blue-400");
        Assert.IsNotNull(cheepElement);

        Console.WriteLine("Cheep element is present");
        // check if public timeline is live
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }).ClickAsync();
        await Expect(Page.Locator("h2")).ToContainTextAsync("Public Timeline");

        Console.WriteLine("Public timeline is live");

        // get the last page
        await Page.GetByRole(AriaRole.Link, new() { Name = "21" }).ClickAsync();

        // check if it is the last page
        await Expect(Page.Locator("body")).ToContainTextAsync("21");

        // check if the last cheep is Hello, BDSA students!
        await Expect(Page.Locator("body")).ToContainTextAsync("Hello, BDSA students!");

        await Expect(Page.Locator("div").Filter(new() { HasText = "Helge 08/01/2023 12:16:48" }).Nth(2)).ToBeVisibleAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("08/01/2023 12:16:48");

        Console.WriteLine("Last cheep is Hello, BDSA students!");

        // check if username is present 
        await Page.GetByRole(AriaRole.Link, new() { Name = "Helge" }).ClickAsync();

        await Page.GetByText("Name: Helge").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("Name: Helge");

        await Page.GetByText("Status: OFFLINE").ClickAsync();

        Console.WriteLine("Username is present");

        // go to helges timeline         
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Helge Profile" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "1", Exact = true }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();

        Console.WriteLine("Helges timeline is live");

        // go to signin
        await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();

        await Page.GetByPlaceholder("Email Address").ClickAsync();

        await Page.GetByPlaceholder("Email Address").FillAsync("chirp@gmail.com");

        await Page.GetByPlaceholder("Password").ClickAsync();

        await Page.GetByPlaceholder("Password").FillAsync("Goqo8003");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();

        Console.WriteLine("Sign in is live");

        // find cheep from a user to check if like works
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine 08/01/2023 13:17:10 Follow Until then I thought it was my" }).Locator("#reactionButton").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("1 Like");

        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine 08/01/2023 13:17:10 Follow Until then I thought it was my" }).Locator("#reactionButton").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("0 Likes");

        Console.WriteLine("Like works");

        // cheep a cheep
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cheep" }).ClickAsync();

        await Page.Locator("#cheepTextArea").FillAsync("hej chirp #chirp");

        await Page.GetByText("Character left:").ClickAsync();

        await Expect(Page.Locator("#charactersLeft")).ToContainTextAsync("Character left: 144");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Cheep!" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "#chirp", Exact = true }).ClickAsync();

        await Expect(Page.Locator("h2")).ToContainTextAsync("Cheeps tagged with #chirp");

        await Expect(Page.Locator("h3")).ToContainTextAsync("Popular Hashtags");

        Console.WriteLine("Cheep a cheep works");

        // check functions on user profile

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("chirppyboi");

        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            Console.WriteLine($"Dialog message: {dialog.Message}");
            dialog.DismissAsync();
            Page.Dialog -= Page_Dialog_EventHandler;
        }
        Page.Dialog += Page_Dialog_EventHandler;
        await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("Followers");

        await Page.GetByText("Following").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("Following");

        Console.WriteLine("User profile functions works");


        // check status function 




        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to OFFLINE" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to UNAVAILABLE" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to ONLINE" }).ClickAsync();

        Console.WriteLine("Status function works");

        // check if follow and unfollow works

        await Page.GotoAsync("https://bdsagroup17chirprazor.azurewebsites.net/");

        await Page.Locator("li").Filter(new() { HasText = "Roger Histand 08/01/2023 13:17:13 Follow I waited for him to the deck, summoned" }).Locator("form").First.ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("Roger Histand 08/01/2023 13:17:13 Unfollow");

        await Page.Locator("li").Filter(new() { HasText = "Roger Histand 08/01/2023 13:17:13 Unfollow I waited for him to the deck," }).GetByRole(AriaRole.Link).ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("1");

        await Page.Locator("li").Filter(new() { HasText = "Roger Histand 08/01/2023 13:17:20 Unfollow You can understand his regarding it" }).Locator("form").First.ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("");

        Console.WriteLine("Follow and unfollow works");


    }
}