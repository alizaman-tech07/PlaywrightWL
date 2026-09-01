using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTest2.Utilities
{
    using Microsoft.Playwright;

    public class AlertHelper
    {
        private readonly IPage _page;

        public AlertHelper(IPage page)
        {
            _page = page;
        }

        public async Task CookiesAlertMethod()
        {
            var acceptCookies = _page.Locator("//*[contains(@class,'cookies-p-btn-accept')]");

            if (await acceptCookies.CountAsync() > 0 &&
                await acceptCookies.First.IsVisibleAsync())
            {
                Console.WriteLine("Cookies popup is displayed: Accept Cookies");

                await acceptCookies.First.ClickAsync();

                Console.WriteLine("Cookies popup: Click Accept Cookies");
            }
            else
            {
                Console.WriteLine("No Cookies popup appeared");
            }
        }
    }
}
