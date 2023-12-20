using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class PlaywrightTests
    {
        private IBrowser browser;
        private IPage Page;

        [SetUp]
        public async Task SetUp()
        {
            var playwright = await Playwright.CreateAsync();
            browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
            });

            var context = await browser.NewContextAsync();
            Page = await context.NewPageAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await browser.CloseAsync();
        }

        [Test]
        public async Task CheckElementPresence()
        {
            if (Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true")
            {
                Assert.Ignore("Test ignored on GitHub Actions");
            }
            else
            {
                // get the website 
                await Page.GotoAsync("https://bdsagroup17chirprazor.azurewebsites.net/");
                Console.WriteLine("Website is live");

                // Assert that the title contains "Chirp!"
                Assert.That(Page.TitleAsync, Does.Match(new Regex("Chirp!")));

                // Check that the "cheep" element is present
                var cheepElement = await Page.QuerySelectorAsync(".text-blue-400");
                Assert.IsNotNull(cheepElement);
                Console.WriteLine("Cheep element is present");

                // check if public timeline is live
                await Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }).ClickAsync();
                Assert.That(await Page.Locator("h2").InnerTextAsync(), Does.Contain("Public Timeline"));
                Console.WriteLine("Public timeline is live");

                // get the last page
                await Page.GetByRole(AriaRole.Link, new() { Name = "21" }).ClickAsync();

                // check if it is the last page
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("21"));

                // check if the last cheep is Hello, BDSA students!
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("Hello, BDSA students!"));

                // Assert that the "Helge 08/01/2023 12:16:48" is visible
                Assert.That(await Page.Locator("div").Filter(new() { HasText = "Helge 08/01/2023 12:16:48" }).Nth(2).IsVisibleAsync(), Is.True);
                Console.WriteLine("Last cheep is Hello, BDSA students!");

                // check if username is present 
                await Page.GetByRole(AriaRole.Link, new() { Name = "Helge" }).ClickAsync();
                await Page.GetByText("Name: Helge").ClickAsync();
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("Name: Helge"));
                await Page.GetByText("Status: OFFLINE").ClickAsync();
                Console.WriteLine("Username is present");

                // go to helges timeline         
                await Page.GetByRole(AriaRole.Heading, new() { Name = "Helge Profile" }).ClickAsync();
                await Page.GetByRole(AriaRole.Link, new() { Name = "1", Exact = true }).ClickAsync();
                await Page.GetByRole(AriaRole.Link, new() { Name = "Public Timeline" }).ClickAsync();
                Console.WriteLine("Helges timeline is live");

                // go to signin --
                await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in" }).ClickAsync();
                await Page.GetByPlaceholder("Email Address").ClickAsync();

                //  note for your own test you can remove this line and login with your own account using github in the playwright browser
                // this is a testing email but if you want to test it yourself you can change it to your own email
                await Page.GetByPlaceholder("Email Address").FillAsync("chirppy@chirp.io");
                await Page.GetByPlaceholder("Password").ClickAsync();

                //  note for your own test you can remove this line and login with your own account using github in the playwright browser
                // this is a testing password but if you want to test it yourself you can change it to your own password
                await Page.GetByPlaceholder("Password").FillAsync("Vuta2325");
                await Page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();
                Console.WriteLine("Sign in is live");

                // find cheep from a user to check if like works
                await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine 08/01/2023 13:17:10 Follow Until then I thought it was my" }).Locator("#reactionButton").ClickAsync();
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("1 Like"));

                await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine 08/01/2023 13:17:10 Follow Until then I thought it was my" }).Locator("#reactionButton").ClickAsync();
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("0 Likes"));
                Console.WriteLine("Like works");

                // cheep a cheep
                await Page.GetByRole(AriaRole.Button, new() { Name = "Cheep" }).ClickAsync();
                await Page.Locator("#cheepTextArea").FillAsync("hej chirp #chirp");
                await Page.GetByText("Character left:").ClickAsync();

                Assert.That(await Page.Locator("#charactersLeft").InnerTextAsync(), Does.Contain("Character left: 144"));
                await Page.GetByRole(AriaRole.Button, new() { Name = "Cheep!" }).ClickAsync();
                await Page.GetByRole(AriaRole.Link, new() { Name = "#chirp", Exact = true }).ClickAsync();

                Assert.That(await Page.Locator("h2").InnerTextAsync(), Does.Contain("Cheeps tagged with #chirp"));
                Assert.That(await Page.Locator("h3").InnerTextAsync(), Does.Contain("Popular Hashtags"));
                Console.WriteLine("Cheep a cheep works");

                // check functions on user profile
                await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("unknown"));

                void Page_Dialog_EventHandler(object sender, IDialog dialog)
                {
                    Console.WriteLine($"Dialog message: {dialog.Message}");
                    dialog.DismissAsync();
                    Page.Dialog -= Page_Dialog_EventHandler;
                }

                Page.Dialog += Page_Dialog_EventHandler;
                await Page.GetByRole(AriaRole.Button, new() { Name = "Forget Me" }).ClickAsync();

                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("Followers"));

                await Page.GetByText("Following").ClickAsync();
                Assert.That(await Page.Locator("body").InnerTextAsync(), Does.Contain("Following"));
                Console.WriteLine("User profile functions works");

                // check status function 
                await Page.GetByRole(AriaRole.Button, new() { Name = "Set to OFFLINE" }).ClickAsync();
                await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

                await Page.GetByRole(AriaRole.Button, new() { Name = "Set to UNAVAILABLE" }).ClickAsync();
                await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();

                await Page.GetByRole(AriaRole.Button, new() { Name = "Set to ONLINE" }).ClickAsync();
                Console.WriteLine("Status function works");

                // check if follow 

                await Page.Locator("li").Filter(new() { HasText = "Roger Histand 08/01/2023 13:17:13 Follow I waited for him to the deck, summoned" }).Locator("form").First.ClickAsync();

                await Page.Locator("li").Filter(new() { HasText = "Roger Histand 08/01/2023 13:17:13 Follow I waited for him to the deck, summoned" }).Locator("form").First.ClickAsync();

                Console.WriteLine("Follow and unfollow works");

                // delete cheep 
                await Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }).ClickAsync();

                await Page.Locator("form").Filter(new() { HasText = "Delete" }).ClickAsync();
            }
        }
    }
}
