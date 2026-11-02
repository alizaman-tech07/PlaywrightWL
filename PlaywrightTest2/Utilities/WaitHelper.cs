using System.Threading.Tasks;

namespace PlaywrightTest2.Utilities
{
    // Playwright locators auto-wait for visibility/clickability on every action, so the
    // Selenium Visible/Clickable helpers are intentionally not ported. Only the explicit
    // "settle" delay (used after smooth-scroll / modal transitions) survives.
    public static class WaitHelper
    {
        public static Task WaitForDelayAsync(int milliseconds = 5000)
        {
            return Task.Delay(milliseconds);
        }
    }
}
