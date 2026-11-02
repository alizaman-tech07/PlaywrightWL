using Microsoft.Playwright;

namespace PlaywrightTest2
{
    public class Tests
    {

        



        
        public async Task OpenGoogle()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

            var page = await browser.NewPageAsync();

            await page.GotoAsync("https://www.google.com");

            await page.WaitForTimeoutAsync(3000);
        }

        public async Task Test001()
        {
            var playwright = await Playwright.CreateAsync();

            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            { Headless = false, SlowMo = 50, Timeout = 80000, });

            var context = await browser.NewContextAsync();

            var page = await context.NewPageAsync();
           // await page.SetViewportSizeAsync(1920, 1080);

            await page.GotoAsync("https://h10.mysuperbutler.com/");

            await page.Locator("//*[contains(text(),'Room Service')]").First.ClickAsync();

            await page.WaitForTimeoutAsync(3000);


        }

    }
}
