using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Pages
{
    /*
     * RoomServicePage
     * ├── Locators (service entry, menu nav, cart buttons, scroll anchors)
     * └── Actions  (fine-grained click helpers + top-level RoomServiceTCAsync flow)
     */
    public class RoomServicePage : BasePage
    {
        // ============================================================
        //  EXISTING LOCATORS — preserved exactly as before
        // ============================================================
        private readonly ILocator roomService;
        private readonly ILocator menuCategory;
        private readonly ILocator itemsModelPopup;
        private readonly ILocator selectMenuItems;
        private readonly ILocator itemsAddToCart;
        private readonly ILocator cartToCheckout;

        // ============================================================
        //  ADDED SCROLL ANCHORS — ported from Selenium RoomServicePage
        // ============================================================
        private readonly ILocator modalOption;
        private readonly ILocator catElementScroll;
        private readonly ILocator elementScroll;
        private readonly ILocator homeElementScroll;

        public RoomServicePage(IPage page) : base(page)
        {
            // ----- Existing locators (unchanged) -----
            roomService = page.GetByText("Room Service", new() { Exact = false }).First;
            menuCategory = page.Locator("//*[@id='ulMenuCategories']/a[1]");
            itemsModelPopup = page.Locator("//*[@id='dvMenu']/section[1]/div[2]/div[1]");
            selectMenuItems = page.Locator("//*[@id='dvMetaItems']/div[1]");
            itemsAddToCart = page.Locator("//*[contains(text(),'Add for')]");
            cartToCheckout = page.Locator(".basket-checkout-btn-wrap");

            // ----- Added scroll anchors (Selenium parity) -----
            modalOption = page.Locator("//div[@id='dvItemOptions']");
            catElementScroll = page.Locator("//*[@id='ulMenuCategories']/a[1]");
            elementScroll = page.Locator("//*[@id='dvMenu']/section[1]/div[2]");
            homeElementScroll = page.Locator("//*[contains(text(),'Our Services')]");
        }

        // ============================================================
        //  EXISTING SHORTCUT — preserved (used by current green test)
        // ============================================================
        public async Task OpenRoomServiceAsync()
        {
            Console.WriteLine("Open Website URL");
            Console.WriteLine("Click on Service");
            await roomService.ClickAsync();
            Console.WriteLine("Click on Menu");
            await menuCategory.ClickAsync();

            Console.WriteLine("Click on Menu Model");
            await itemsModelPopup.ClickAsync();

            Console.WriteLine("Click on Menu Items");
            await selectMenuItems.ClickAsync();

            Console.WriteLine("Click on Add Items into Cart");
            await itemsAddToCart.ClickAsync();

            Console.WriteLine("Click on Move to Checkout");
            await cartToCheckout.ClickAsync();
        }

        // ============================================================
        //  ADDED — service lookup helpers (Selenium parity)
        // ============================================================

        public ILocator ServiceByName(string serviceName)
        {
            return Page.Locator($"//*[contains(text(),'{serviceName}')]");
        }

        public async Task SelectCategoryAndServiceAsync(ILocator categoryLocator, string serviceName)
        {
            // 1. Click the category.
            await categoryLocator.ClickAsync();
            Console.WriteLine("Clicked on Category.");

            // 2. Decide whether a result list is shown or if we were sent directly to the service page.
            //    Selenium RoomServicePage uses a 3-second settle here (HomePage uses 2s).
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
            ILocator specificService = ServiceByName(serviceName);
            await specificService.First.ClickAsync();
        }

        // ============================================================
        //  ADDED — fine-grained menu / cart actions
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
        //  ADDED — HomePage lifecycle shortcut mirrored from Selenium
        //  RoomServicePage.HomePageService (same name/body as HomePage).
        //  Kept because the Selenium Express flow calls it on the
        //  service-page object, not on HomePage.
        // ============================================================

        //public async Task HomePageServiceAsync()
        //{
        //    var alertModel = new AlertHelper(Page);

        //    await alertModel.ModalPopupMethodAsync();
        //    await alertModel.PopupAlertMethodAsync();
        //    await alertModel.CookiesAlertMethodAsync();

        //    await ScrollPageAsync(homeElementScroll, "Scroll Down for Category Services : ElementScroll Click");
        //}

        // ============================================================
        //  ADDED — Top-level Room Service flow
        //  Mirrors Selenium RoomServicePage.RoomServiceTC step-for-step.
        // ============================================================

        public async Task RoomServiceTCAsync()
        {
            Console.WriteLine("Scroll Down for Category");
            await ScrollPageAsync(catElementScroll, "Scroll Down for Category : ElementScroll Click");

            Console.WriteLine("Select Category For Items");
            await SelectCategoryClickAsync();

            // Selenium's RoomServiceTC calls WaitHelper.WaitForDelay() with no arg — default 5s.
            await WaitHelper.WaitForDelayAsync();

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
