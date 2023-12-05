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

        // Expect a title to have title Chirp!.
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));

        // Check that the "cheep" element is present
        var cheepElement = await Page.QuerySelectorAsync(".text-blue-400");
        Assert.IsNotNull(cheepElement);

        // Get the text of the "cheep" element
        string cheepText = await cheepElement.InnerTextAsync();
        Assert.That(cheepText, Is.EqualTo("Hello josh"));

        // i want to see if the page has a url is present 
        var checkLink = await Page.GetByRole(AriaRole.Link, new() { Name = "hananinas" }).IsVisibleAsync();

        Assert.True(checkLink);

        // check if the link is clickable
        await Page.GetByRole(AriaRole.Link, new() { Name = "hananinas" }).ClickAsync();




    }
}