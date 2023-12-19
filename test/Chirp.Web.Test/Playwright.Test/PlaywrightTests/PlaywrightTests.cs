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

        if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
        {
            Assert.Ignore("GitHub Actions");
        }
        // get the website 
        await Page.GotoAsync("https://bdsagroup17chirprazor.azurewebsites.net/");

        // Expect a title to have title Chirp!.
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));

        // Check that the "cheep" element is present
        var cheepElement = await Page.QuerySelectorAsync(".text-blue-400");
        Assert.IsNotNull(cheepElement);

        // check if public timeline is live
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }).ClickAsync();
        await Expect(Page.Locator("h2")).ToContainTextAsync("Public Timeline");

        // get the last page
        await Page.GetByRole(AriaRole.Link, new() { Name = "21" }).ClickAsync();

        // check if it is the last page
        await Expect(Page.Locator("body")).ToContainTextAsync("21");

        // check if the last cheep is Hello, BDSA students!
        await Expect(Page.Locator("body")).ToContainTextAsync("Hello, BDSA students!");

        await Expect(Page.Locator("div").Filter(new() { HasText = "Helge 08/01/2023 12:16:48" }).Nth(2)).ToBeVisibleAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("08/01/2023 12:16:48");

        // check if username is present 
        await Page.GetByRole(AriaRole.Link, new() { Name = "Helge" }).ClickAsync();

        await Page.GetByText("Name: Helge").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("Name: Helge");

        await Page.GetByText("Status: OFFLINE").ClickAsync();

        // go to helges timeline         
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Helge Profile" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "1", Exact = true }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();

        // go to signin
        await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();

        await Page.GetByPlaceholder("Email Address").ClickAsync();

        await Page.GetByPlaceholder("Email Address").FillAsync("chirp@gmail.com");

        await Page.GetByPlaceholder("Password").ClickAsync();

        await Page.GetByPlaceholder("Password").FillAsync("Goqo8003");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();

        // find cheep from a user to check if like works
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine 08/01/2023 13:17:10 Follow Until then I thought it was my" }).Locator("#reactionButton").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("1 Like");

        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine 08/01/2023 13:17:10 Follow Until then I thought it was my" }).Locator("#reactionButton").ClickAsync();

        await Expect(Page.Locator("body")).ToContainTextAsync("0 Likes");

        // cheep a cheep
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cheep" }).ClickAsync();

        await Page.Locator("#cheepTextArea").FillAsync("hej chirp #chirp");

        await Page.GetByText("Character left:").ClickAsync();

        await Expect(Page.Locator("#charactersLeft")).ToContainTextAsync("Character left: 144");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Cheep!" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "#chirp", Exact = true }).ClickAsync();

        await Expect(Page.Locator("h2")).ToContainTextAsync("Cheeps tagged with #chirp");

        await Expect(Page.Locator("h3")).ToContainTextAsync("Popular Hashtags");

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


        // check status function 

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to ONLINE" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to OFFLINE" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to UNAVAILABLE" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Set to ONLINE" }).ClickAsync();

    }
}