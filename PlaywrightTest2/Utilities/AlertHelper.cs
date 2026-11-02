using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace PlaywrightTest2.Utilities
{
    /// <summary>
    /// Dismisses DOM-based banners that may appear during the home-page flow:
    ///   1. gift / welcome interstitial modal
    ///   2. browser-notification allow/deny banner
    ///   3. cookie-consent banner
    ///
    /// These are HTML overlays owned by the site (not native browser dialogs),
    /// so Playwright's IPage.Dialog / Dialog.AcceptAsync / Dialog.DismissAsync
    /// APIs do not apply here. All detection is locator-based with a visibility
    /// probe up-front, so a missing banner costs microseconds instead of 30 s
    /// of auto-wait.
    /// </summary>
    public class AlertHelper
    {
        private readonly IPage _page;

        public AlertHelper(IPage page)
        {
            _page = page;
        }

        /// <summary>
        /// Gift / welcome interstitial. Mirrors Selenium AlertHelper.ModalPopupMethod,
        /// but checks visibility before clicking (Playwright-idiomatic) so a missing
        /// popup doesn't block for Playwright's default 30 s auto-wait.
        /// </summary>
        public async Task ModalPopupMethodAsync()
        {
            var test = ExtentTestManager.TryGetTest();

            var giftPopup = _page.Locator("//*[contains(@id,'dvGenericModel')] //*[contains(@class,'gift-cross')]");
            var giftPopupWelcome = _page.Locator("//*[contains(@id,'welcomeHotel')] //*[contains(@class,'gift-cross')]");

            if (await giftPopup.CountAsync() > 0 && await giftPopup.First.IsVisibleAsync())
            {
                Console.WriteLine("Modal popup is displayed : Continue POPUP");
                test?.Info("Interstitials Popup is Display");
                await giftPopup.First.ClickAsync();
            }
            else if (await giftPopupWelcome.CountAsync() > 0 && await giftPopupWelcome.First.IsVisibleAsync())
            {
                Console.WriteLine("Modal popup is displayed : Continue GIFT");
                test?.Info("Interstitials Welcome Popup is Display");
                await giftPopupWelcome.First.ClickAsync();
            }
            else
            {
                Console.WriteLine("No popup appeared");
            }
        }

        /// <summary>
        /// Browser-notification prompt (deny / dismiss). Mirrors Selenium
        /// AlertHelper.PopupAlertMethod. Selenium's version clicks blindly;
        /// we guard with a visibility probe so repeat sessions (where the layer
        /// is already dismissed) don't incur 30 s auto-wait timeouts.
        /// </summary>
        public async Task PopupAlertMethodAsync()
        {
            var test = ExtentTestManager.TryGetTest();

            var notNowNotificationIcon = _page.Locator("//*[contains(@data-ga-event,'notification_deny_click')]");
            var dontAllowNotification = _page.Locator("//*[contains(@data-ga-event,'notification_dismiss_confirm_click')]");

            Console.WriteLine("Click on Notification Allow or ( X ) ");

            if (await notNowNotificationIcon.CountAsync() > 0 && await notNowNotificationIcon.First.IsVisibleAsync())
            {
                test?.Info("Click on X icon Notification");
                //await notNowNotificationIcon.First.ClickAsync();
                await ActionHelper.ClickAsync(notNowNotificationIcon, "Click on X icon Notification");
            }

            if (await dontAllowNotification.CountAsync() > 0 && await dontAllowNotification.First.IsVisibleAsync())
            {
                test?.Info("Click on Dont Allow Button");
                await dontAllowNotification.First.ClickAsync();
            }
        }

        /// <summary>
        /// Cookie-consent banner. Mirrors Selenium AlertHelper.CookiesAlertMethod.
        /// Renamed from CookiesAlertMethod to CookiesAlertMethodAsync to match the
        /// Async-suffix convention used by the three Page callers.
        /// </summary>
        public async Task CookiesAlertMethodAsync()
        {
            var test = ExtentTestManager.TryGetTest();

            var acceptCookies = _page.Locator("//*[contains(@class,'cookies-p-btn-accept')]");

            if (await acceptCookies.CountAsync() > 0 && await acceptCookies.First.IsVisibleAsync())
            {
                Console.WriteLine("Cookies popup is displayed : Accept Cookies");
                test?.Info("Cookies popup is displayed : Click sAccept Cookies");
                //await acceptCookies.First.ClickAsync();
                await ActionHelper.ClickAsync(acceptCookies, "Cookies popup is displayed : Click sAccept Cookies");
                Console.WriteLine("Cookies popup: Click Accept Cookies");
            }
            else
            {
                Console.WriteLine("No Cookies popup appeared");
            }
        }
    }
}
