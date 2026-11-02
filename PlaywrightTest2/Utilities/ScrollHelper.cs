using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTest2.Utilities
{
    public static class ScrollHelper
    {
        // Pixel-based smooth scroll (page-level). Mirrors the Selenium JS smooth-scroll
        // and keeps the 3-second settle so animations finish before the next action.
        public static async Task SmoothScrollDownAsync(IPage page, int yPixels)
        {
            await page.EvaluateAsync(
                "y => window.scrollBy({ top: y, left: 0, behavior: 'smooth' })",
                yPixels);
            await WaitHelper.WaitForDelayAsync(3000);
        }

        // Scrolls a specific element into view (replaces the JS scrollIntoView helper).
        public static async Task ScrollPageElementsAsync(ILocator element)
        {
            await element.ScrollIntoViewIfNeededAsync();
            await WaitHelper.WaitForDelayAsync(3000);
        }
    }
}
