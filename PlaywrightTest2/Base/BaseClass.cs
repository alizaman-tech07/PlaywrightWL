using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTest2.Base
{
    //Browser, Context aur Page yahan manage honge.
    /*
        BaseTest
    │
    ├── Create Playwright
    ├── Launch Browser
    ├── Create Context
    ├── Create Page
    └── Close everything
    Test class mein browser setup repeat nahi hoga.
    */

    public class BaseTest
    {
        protected IPlaywright Playwright = null!;
        protected IBrowser Browser = null!;
        protected IBrowserContext Context = null!;
        protected IPage Page = null!;
        protected string BaseUrl = string.Empty; //1. appsettings

        [SetUp]
        public async Task Setup()
        {
            //2. START - appsettings.json settings
            var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false).Build();

            BaseUrl = configuration["BaseUrl"] ?? throw new Exception( "BaseUrl not found in appsettings.json");

            //END - appsettings.json settings

            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            Browser = await Playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 50
                });

            Context = await Browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize
                    {
                        //Width = 1920,
                        //Height = 1080
                        Width = 1280,
                        Height = 600
                    }
                });

            Page = await Context.NewPageAsync();

            //3. appsettings
            await Page.GotoAsync(BaseUrl);
        }

        [TearDown]
        public async Task TearDown()
        {
            await Context.CloseAsync();
            await Browser.CloseAsync();

            Playwright.Dispose();
        }
    }
}