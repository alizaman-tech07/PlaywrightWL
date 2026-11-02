using System;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using Microsoft.Playwright;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Base
{
    /*
     * BasePage
     * ├── Locators (defined in subclasses)
     * ├── Actions  (direct Playwright + logging-aware wrappers below)
     * └── Logging  (Extent step info/pass/fail + inline step screenshots)
     */
    public class BasePage
    {
        protected readonly IPage Page;
        protected readonly ExtentTest? test;

        public BasePage(IPage page)
        {
            Page = page;
            test = ExtentTestManager.TryGetTest();
        }

        // Keep existing simple wrappers used by current code — do NOT change signatures.
        public async Task NavigateAsync(string url)
        {
            await Page.GotoAsync(url);
        }

        public async Task FillAsync(ILocator locator, string text)
        {
            await locator.FillAsync(text);
        }

        public async Task<bool> IsVisibleAsync(ILocator locator)
        {
            return await locator.IsVisibleAsync();
        }

        // Logging-aware action wrappers. Mirror the Selenium BasePage API 1:1
        // (step name string arguments identical) so the Extent report reads the same.

        protected async Task ClickAsync(ILocator locator, string stepName)
        {
            try
            {
                test?.Info("Click: " + stepName);
                await locator.ClickAsync();
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Pass("Clicked: " + stepName)
                    .AddScreenCaptureFromBase64String(base64);
            }
            catch (Exception ex)
            {
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Fail("Click failed: " + stepName + " | " + ex.Message)
                    .AddScreenCaptureFromBase64String(base64);
                throw;
            }
        }

        protected async Task VisibleAsync(ILocator locator, string stepName)
        {
            try
            {
                test?.Info("Visible: " + stepName);
                await locator.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Pass("Visible: " + stepName)
                    .AddScreenCaptureFromBase64String(base64);
            }
            catch (Exception ex)
            {
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Fail("Visible failed: " + stepName + " | " + ex.Message)
                    .AddScreenCaptureFromBase64String(base64);
                throw;
            }
        }

        protected async Task EnterTextAsync(ILocator locator, string text, string stepName)
        {
            try
            {
                test?.Info("Enter text: " + stepName);
                await locator.FillAsync(text);
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Pass("Text entered: " + stepName)
                    .AddScreenCaptureFromBase64String(base64);
            }
            catch (Exception ex)
            {
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Fail("Text failed: " + stepName + " | " + ex.Message)
                    .AddScreenCaptureFromBase64String(base64);
                throw;
            }
        }

        protected async Task ScrollAsync(int pixels, string stepName)
        {
            try
            {
                test?.Info("Scroll: " + stepName);
                await ScrollHelper.SmoothScrollDownAsync(Page, pixels);
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Pass("Scrolled: " + stepName)
                    .AddScreenCaptureFromBase64String(base64);
            }
            catch (Exception ex)
            {
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Fail("Scroll failed: " + stepName + " | " + ex.Message)
                    .AddScreenCaptureFromBase64String(base64);
                throw;
            }
        }

        protected async Task ScrollPageAsync(ILocator element, string stepName)
        {
            try
            {
                test?.Info("Scroll: " + stepName);
                await ScrollHelper.ScrollPageElementsAsync(element);
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Pass("Scrolled: " + stepName)
                    .AddScreenCaptureFromBase64String(base64);
            }
            catch (Exception ex)
            {
                string base64 = await TakeStepScreenshotAsync(stepName);
                test?.Fail("Scroll failed: " + stepName + " | " + ex.Message)
                    .AddScreenCaptureFromBase64String(base64);
                throw;
            }
        }

        protected async Task LogStepAsync(string stepName)
        {
            string base64 = await TakeStepScreenshotAsync(stepName);
            test?.Info(stepName)
                .AddScreenCaptureFromBase64String(base64);
        }

        private async Task<string> TakeStepScreenshotAsync(string stepName)
        {
            var screenshot = new ScreenshotHelper(Page);
            return await screenshot.TakeScreenshotAsBase64Async(stepName.Replace(" ", "_"));
        }
    }
}
