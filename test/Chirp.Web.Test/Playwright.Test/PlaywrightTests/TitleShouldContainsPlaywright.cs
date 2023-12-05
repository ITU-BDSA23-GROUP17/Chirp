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
        await Page.GotoAsync("https://bdsagroup17chirprazor.azurewebsites.net/");

        // Check that the "cheep" element is present
        var cheepElement = await Page.QuerySelectorAsync(".text-blue-400");
        Assert.IsNotNull(cheepElement);
    }


}