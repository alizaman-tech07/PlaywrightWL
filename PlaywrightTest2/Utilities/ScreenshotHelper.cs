using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTest2.Utilities
{
    public class ScreenshotHelper
    {
        private readonly IPage _page;

        public ScreenshotHelper(IPage page)
        {
            _page = page;
        }

        /// <summary>
        /// Captures a full-page screenshot, writes a PNG to TestResults/Screenshots for the
        /// CI artifact and local debugging, and returns the Base64-encoded PNG so callers
        /// can embed it inline into the ExtentReport via AddScreenCaptureFromBase64String.
        /// </summary>
        public async Task<string> TakeScreenshotAsBase64Async(string testName)
        {
            string root = PathHelper.GetRoot();
            string screenshotFolder = Path.Combine(root, "Screenshots");
            Directory.CreateDirectory(screenshotFolder);

            string safeName = string.Concat(testName.Split(Path.GetInvalidFileNameChars()));
            string fileName = $"{safeName}_{DateTime.Now:yyyyMMdd_HHmmssfff}.png";
            string fullPath = Path.Combine(screenshotFolder, fileName);

            byte[] bytes = await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = fullPath,
                FullPage = false
            });

            return Convert.ToBase64String(bytes);
        }
    }
}
