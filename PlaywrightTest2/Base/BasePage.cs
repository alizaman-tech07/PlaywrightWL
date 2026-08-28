using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTest2.Base
{
 /*
     * RoomServicePage
│
├── Locators
└── Actions
     */
    public class BasePage
    {
        protected readonly IPage Page;

        public BasePage(IPage page)
        {
            Page = page;
        }

        public async Task NavigateAsync(string url)
        {
            await Page.GotoAsync(url);
        }

        public async Task ClickAsync(ILocator locator)
        {
            await locator.ClickAsync();
        }

        public async Task FillAsync(ILocator locator, string text)
        {
            await locator.FillAsync(text);
        }

        public async Task<bool> IsVisibleAsync(ILocator locator)
        {
            return await locator.IsVisibleAsync();
        }
    }
}
//Common page functionality yahan rakho.
//Abhi simple rakho. Har Playwright method ka wrapper banane ki zarurat nahi.