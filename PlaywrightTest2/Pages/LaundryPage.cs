using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Pages
{
    /*
     * LaundryPage
     * ├── Locators (menu nav, cart buttons, scroll anchors)
     * └── Actions  (fine-grained click helpers + top-level LaundryTCAsync flow)
     */
    public class LaundryPage : BasePage
    {
        // ============================================================
        //  Locators — ported 1:1 from Selenium LaundryPage.cs
        // ============================================================
        private readonly ILocator menuCategory;
        private readonly ILocator itemsModelPopup;
        private readonly ILocator selectMenuItems;
        private readonly ILocator itemsAddToCart;
        private readonly ILocator cartToCheckout;

        private readonly ILocator modalOption;
        private readonly ILocator catElementScroll;
        private readonly ILocator elementScroll;
        private readonly ILocator homeElementScroll;

        public LaundryPage(IPage page) : base(page)
        {
            menuCategory = page.Locator("//*[@id='ulMenuCategories']/a[1]");
            itemsModelPopup = page.Locator("//*[@id='dvMenu']/section[1]/div[2]/div[1]");
            selectMenuItems = page.Locator("//*[@id='dvMetaItems']/div[1]");
            itemsAddToCart = page.Locator("//*[contains(text(),'Add for')]");
            cartToCheckout = page.Locator(".basket-checkout-btn-wrap");

            modalOption = page.Locator("//div[@id='dvItemOptions']");
            catElementScroll = page.Locator("//*[@id='ulMenuCategories']/a[1]");
            elementScroll = page.Locator("//*[@id='dvMenu']/section[1]/div[2]");
            homeElementScroll = page.Locator("//*[contains(text(),'Our Services')]");
        }

        // ============================================================
        //  Category / service lookup
        //  Mirrors Selenium LaundryPage.SelectCategoryAndService / SelectServiceFromList.
        //  Caller supplies the category ILocator (Selenium passes a By).
        //  3-second settle preserved — Selenium LaundryPage uses Thread.Sleep(3000).
        // ============================================================

        public async Task SelectCategoryAndServiceAsync(ILocator categoryLocator, string serviceName)
        {
            await categoryLocator.ClickAsync();
            Console.WriteLine("Clicked on Category.");

            try
            {
                var services = Page.Locator(".restaurantSearchResult");
                await WaitHelper.WaitForDelayAsync(3000);

                int count = await services.CountAsync();
                if (count > 0)
                {
                    Console.WriteLine($"{count} service(s) found in list. Searching for: {serviceName}");
                    await SelectServiceFromListAsync(serviceName);
                }
                else
                {
                    Console.WriteLine("No list found. Assuming direct service page opened.");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Direct redirection detected or element not found.");
            }
        }

        public async Task SelectServiceFromListAsync(string serviceName)
        {
            ILocator specificService = Page.Locator($"//*[contains(text(),'{serviceName}')]");
            await specificService.First.ClickAsync();
        }

        // ============================================================
        //  Fine-grained menu / cart actions.
        //  Each wrapper logs via BasePage.ClickAsync so the Extent step
        //  names match the Selenium report 1:1.
        // ============================================================

        public async Task SelectCategoryClickAsync() =>
            await ClickAsync(menuCategory, "Select Category");

        public async Task SelectItemModelPopupClickAsync() =>
            await ClickAsync(itemsModelPopup, "Click on Items");

        public async Task SelectItemClickAsync() =>
            await ClickAsync(selectMenuItems, "Select Item Variation");

        public async Task SelectItemAddToCartClickAsync() =>
            await ClickAsync(itemsAddToCart, "Click on Add Button");

        public async Task CartToCheckoutClickAsync() =>
            await ClickAsync(cartToCheckout, "Continue to Checkout Page");

        // ============================================================
        //  HomePage lifecycle shortcut (same shape as HomePage / RoomServicePage).
        //  Mirrors Selenium LaundryPage.HomePageService — note the extra
        //  "Scroll Down for Category Service" Console log that is unique
        //  to the LaundryPage copy in the Selenium source.
        // ============================================================

        public async Task HomePageServiceAsync()
        {
            var alertModel = new AlertHelper(Page);

            await alertModel.ModalPopupMethodAsync();
            await alertModel.PopupAlertMethodAsync();
            await alertModel.CookiesAlertMethodAsync();

            Console.WriteLine("Scroll Down for Category Service");

            await ScrollPageAsync(homeElementScroll, "Scroll Down for Category Services : ElementScroll Click");
        }

        // ============================================================
        //  Top-level Laundry flow — mirrors Selenium LaundryPage.LaundryTC
        //  step-for-step. Note Selenium LaundryTC has NO WaitForDelay
        //  between SelectCategoryClick and the next scroll (RoomServiceTC does);
        //  the difference is preserved.
        // ============================================================

        public async Task LaundryTCAsync()
        {
            Console.WriteLine("Scroll Down for Category");
            await ScrollPageAsync(catElementScroll, "Scroll Down for Category : ElementScroll Click");

            Console.WriteLine("Select Category For Items");
            await SelectCategoryClickAsync();

            Console.WriteLine("Scroll Down for Items : ElementScroll Click");
            await ScrollPageAsync(elementScroll, "Scroll Down for Items : ElementScroll Click");

            Console.WriteLine("Select Items and Open Model for items selection");
            await LogStepAsync("Select Items and Open Model for items selection");
            await SelectItemModelPopupClickAsync();

            Console.WriteLine("Scroll Down for Items : ElementScroll Click");
            await ScrollPageAsync(modalOption, "Scroll Down for Items : ElementScroll Click");

            Console.WriteLine("Select Items");
            await LogStepAsync("Select Items");
            await SelectItemClickAsync();

            Console.WriteLine("Select Items and Add into Cart");
            await LogStepAsync("Select Items and Add into Cart");
            await SelectItemAddToCartClickAsync();

            Console.WriteLine("Click to Continue for Checkout Page");
            await LogStepAsync("Click to Continue for Checkout Page");
            await CartToCheckoutClickAsync();
        }
    }
}
