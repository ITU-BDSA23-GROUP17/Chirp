using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

public class TitleShouldContainsPlaywright : PageTest
{
    [TestMethod]
    public async Task Apply_On_Carreer_Site()
    {
        await Page.GotoAsync("https://playwright.dev");

        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));
    }
}