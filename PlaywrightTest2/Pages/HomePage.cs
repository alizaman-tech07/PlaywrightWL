using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Pages
{
    public class HomePage : BasePage
    {
        // Scroll anchor on the home page ("Our Services" section header).
        private readonly ILocator homeElementScroll;

        // QR skip modal
        private readonly ILocator qrImage;
        private readonly ILocator qrskipNo;
        private readonly ILocator verifyBtn;

        public HomePage(IPage page) : base(page)
        {
            homeElementScroll = page.Locator("//*[contains(text(),'Our Services')]");
            qrImage = page.Locator("//img[@class='qrp-logo']");
            qrskipNo = page.Locator("//*[@id='dvBypassModal']/div[1]/div[2]/div //*[@id='txtBypassPin']");
            verifyBtn = page.Locator("//button[contains(text(),'Verify')]");
        }

        // ============================================================
        //  QR skip handling
        //  Mirrors Selenium HomePage: a LogStep "QR is ON" is written
        //  unconditionally before the visibility check (Selenium parity;
        //  the message always logs even when QR is actually off).
        // ============================================================

        public async Task<bool> IsQRAvailableAsync()
        {
            await LogStepAsync("QR is ON");
            Console.WriteLine("*QR is ON");

            if (await qrImage.CountAsync() == 0)
                return false;

            return await qrImage.First.IsVisibleAsync();
        }

        public async Task ClickImageSevenTimesAsync()
        {
            for (int i = 0; i < 7; i++)
            {
                await qrImage.First.ClickAsync();
            }
        }

        public async Task EnterQRskipNoAsync(string qrskipNumber)
        {
            await EnterTextAsync(qrskipNo, qrskipNumber, "Enter QRskip");
        }

        public async Task ClickVerifyBtnAsync()
        {
            await ClickAsync(verifyBtn, "Click on Verify Button");
        }

        public async Task FillQRpinAsync(string qrNumber)
        {
            Console.WriteLine("*Enter QR Number");
            await EnterQRskipNoAsync(qrNumber);
            await WaitHelper.WaitForDelayAsync(2000);
            await ClickVerifyBtnAsync();
        }

        // ============================================================
        //  Dynamic service lookup
        //  Mirrors Selenium ServiceByName / SelectCategoryAndService / SelectServiceFromList.
        //  Caller passes a category ILocator (same shape as Selenium passing a By).
        // ============================================================

        public ILocator ServiceByName(string serviceName)
        {
            return Page.Locator($"//*[contains(text(),'{serviceName}')]");
        }

        public async Task SelectCategoryAndServiceAsync(ILocator categoryLocator, string serviceName)
        {
            // 1. Click the category
            await categoryLocator.ClickAsync();
            Console.WriteLine("Clicked on Category.");

            // 2. Determine if a result list is shown or if the category navigated directly
            //    to the service page.
            try
            {
                var services = Page.Locator(".restaurantSearchResult");
                await WaitHelper.WaitForDelayAsync(2000);

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
        //  Home page lifecycle: dismiss all three alerts, then scroll to
        //  the "Our Services" category grid.
        // ============================================================

        public async Task HomePageServiceAsync()
        {
            var alertModel = new AlertHelper(Page);

            await alertModel.ModalPopupMethodAsync();
            await alertModel.PopupAlertMethodAsync();
            await alertModel.CookiesAlertMethodAsync();

            await ScrollPageAsync(homeElementScroll, "Scroll Down for Category Services : ElementScroll Click");
        }
    }
}
