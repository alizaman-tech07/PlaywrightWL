using System;
using AventStack.ExtentReports;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Base
{
    /*
        BaseTest
    │
    ├── [OneTimeSetUp]  start Extent report
    ├── [SetUp]         create Playwright/Browser/Context/Page + create Extent test + navigate
    ├── [TearDown]      log pass/fail + screenshot on failure + dispose Playwright
    └── [OneTimeTearDown] flush Extent (email send is opt-in via env vars)
    */

    public class BaseTest
    {
        protected IPlaywright Playwright = null!;
        protected IBrowser Browser = null!;
        protected IBrowserContext Context = null!;
        protected IPage Page = null!;
        protected string BaseUrl = string.Empty;

        private static ExtentReports _extent = null!;

        [OneTimeSetUp]
        public void StartReport()
        {
            _extent = ExtentManager.GetInstance();
        }

        [SetUp]
        public async Task Setup()
        {
            // Create the Extent test node for this NUnit test (thread-local).
            ExtentTestManager.CreateTest(TestContext.CurrentContext.Test.Name);

            // Base URL + title from Excel Environment sheet (parity with Selenium project).
            // Row 2 = Staging (col 2 = url, col 3 = title). Row 3 = Live.
            BaseUrl = ExcelHelper.GetCell("Environment", 2, 2);
            string titleName = ExcelHelper.GetCell("Environment", 2, 3);

            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new Exception("Base URL not found in TestData.xlsx -> Environment sheet, row 2 col 2.");

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
                        Width = 1280,
                        Height = 600
                    }
                });

            Page = await Context.NewPageAsync();

            await Page.GotoAsync(BaseUrl);

            Assert.That(Page.Url, Does.Contain("https"));
        }

        [TearDown]
        public async Task TearDown()
        {
            var test = ExtentTestManager.TryGetTest();
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (test != null && Page != null)
            {
                if (status == TestStatus.Failed)
                {
                    var screenshot = new ScreenshotHelper(Page);
                    string base64 = await screenshot.TakeScreenshotAsBase64Async(TestContext.CurrentContext.Test.Name);

                    test.Fail("Test Failed")
                        .AddScreenCaptureFromBase64String(base64);
                }
                else if (status == TestStatus.Passed)
                {
                    test.Pass("Test Passed");
                }
            }

            if (Context != null) await Context.CloseAsync();
            if (Browser != null) await Browser.CloseAsync();
            Playwright?.Dispose();
        }

        [OneTimeTearDown]
        public void EndReport()
        {
            _extent?.Flush();

            // Email delivery is opt-in via environment variables (see EmailHelper).
            // Enable by uncommenting the next block once REPORT_EMAIL_* vars are provisioned.
            //
            // string reportPath = System.IO.Path.Combine(PathHelper.GetRoot(), "ExtentReport.html");
            // if (System.IO.File.Exists(reportPath))
            // {
            //     EmailHelper.SendReport(reportPath);
            // }
            // else
            // {
            //     Console.WriteLine("Email error: Report file not found at " + reportPath);
            // }
        }
    }
}
