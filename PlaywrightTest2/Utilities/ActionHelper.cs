using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTest2.Utilities
{
    /// <summary>
    /// Static, logging-aware click helper for utility contexts that only hold an
    /// ILocator (e.g. AlertHelper, future popup/consent helpers) and therefore
    /// cannot use BasePage.ClickAsync (which is a protected instance method).
    ///
    /// Semantics mirror the Selenium ActionHelper.Click 1:1:
    ///   Info -> ClickAsync -> Pass on success.
    ///   On failure: capture Base64 screenshot, Fail with it, rethrow.
    /// Unlike BasePage.ClickAsync, this does NOT snap a screenshot on success —
    /// that distinction is intentional parity with the Selenium source and keeps
    /// the report concise for high-frequency utility clicks.
    ///
    /// Playwright's locator auto-waits for actionability, so the Selenium
    /// WaitHelper.Clickable call is not ported.
    /// </summary>
    public static class ActionHelper
    {
        public static async Task ClickAsync(ILocator locator, string stepName)
        {
            var test = ExtentTestManager.TryGetTest();

            try
            {
                test?.Info("Click: " + stepName);

                await locator.ClickAsync();

                test?.Pass("Clicked: " + stepName);
            }
            catch (Exception ex)
            {
                var screenshot = new ScreenshotHelper(locator.Page);
                string base64 = await screenshot.TakeScreenshotAsBase64Async(stepName);

                test?.Fail("Click failed: " + stepName + " | " + ex.Message)
                    .AddScreenCaptureFromBase64String(base64);

                throw;
            }
        }
    }
}
