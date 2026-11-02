using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest2.Base;

namespace PlaywrightTest2.Pages
{
    public class ReviewPage : BasePage
    {
        // ============================================================
        //  Locators — ported 1:1 from Selenium ReviewPage.cs
        //  (Note: the ReviewPage locator for successReviewHotel checks
        //   for "Review submitted", distinct from CheckoutPage's
        //   "Thank You" variant — both preserved in their original files.)
        // ============================================================
        private readonly ILocator successReviewHotel;
        private readonly ILocator orderReview;
        private readonly ILocator textReview;
        private readonly ILocator btnReviewSubmit;
        private readonly ILocator orderReviewUpdate;
        private readonly ILocator btnCancel;
        private readonly ILocator btnUpdate;
        private readonly ILocator btnOkay;
        private readonly ILocator btnClose;

        public ReviewPage(IPage page) : base(page)
        {
            successReviewHotel = page.Locator("//*[@id='enterpriseReviewModal']/div/div/div[4] //strong[contains(text(),'Review submitted')]");
            orderReview = page.Locator("#ctl00_ContentPlaceHolder1_dvRateYourExperience");
            textReview = page.Locator("//*[@id='enterpriseReviewModal']/div/div/div[2]/div//textarea");
            btnReviewSubmit = page.Locator("#btnEnterpriseReviewsSave");
            orderReviewUpdate = page.Locator("//*[@id='enterpriseReviewModal']  //div[contains(text(),'Seems like you have previously reviewed')]");
            btnCancel = page.Locator("//*[@id='enterpriseReviewModal']  //button[contains(text(),'Cancel')]");
            btnUpdate = page.Locator("#btnEnterpriseReviewsSave");
            btnOkay = page.Locator("#spClose");
            btnClose = page.Locator("//*[@id='enterpriseReviewModal']/div/div[1]/div //button[@class='close']");
        }

        // ============================================================
        //  Click the "Rate Your Experience" tile to open the review modal.
        //  Mirrors Selenium ReviewPage.SuccessOrderReview.
        // ============================================================
        public async Task SuccessOrderReviewAsync()
        {
            await ClickAsync(orderReview, "Click For Order Review");
        }

        // ============================================================
        //  Read the "Review submitted" success text from the modal footer.
        //  Mirrors Selenium ReviewPage.GetSuccessMessageReview:
        //    WaitHelper.Visible(driver, successReviewHotel).Text
        // ============================================================
        public async Task<string> GetSuccessMessageReviewAsync()
        {
            await successReviewHotel.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });
            return await successReviewHotel.InnerTextAsync();
        }

        // ============================================================
        //  Fire the full review submission flow.
        //  Mirrors Selenium ReviewPage.HandleReviewSubmission step-for-step:
        //    1. Enter review text
        //    2. Click Submit
        //    3. If the "previously reviewed" popup appears, click Update
        //    4. Wait for the success popup, assert "Review submitted",
        //       then close it.
        // ============================================================
        public async Task HandleReviewSubmissionAsync(string reviewText)
        {
            // Step 1: enter review text and submit.
            await EnterTextAsync(textReview, reviewText, "Entering review text.");
            await ClickAsync(btnReviewSubmit, "Click Submit");

            // Step 2: duplicate / update popup check.
            if (await IsDuplicateReviewPopupVisibleAsync())
            {
                Console.WriteLine("Duplicate detected: Clicking Update.");
                await ClickAsync(btnUpdate, "Click Update");
            }

            // Step 3: wait for the success popup, then assert + close.
            //         Selenium calls WaitHelper.Visible(..).Displayed — if the
            //         element never becomes visible, that call throws and the
            //         block is skipped. Playwright's WaitForAsync has the same
            //         "throw on timeout" shape, so reaching the body implies
            //         the element is visible.
            await btnClose.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 15000
            });

            // Assert first, close second — same order as Selenium.
            string actualMessage = await GetSuccessMessageReviewAsync();
            Assert.That(actualMessage, Does.Contain("Review submitted"), "Success message mismatch!");
            Console.WriteLine("Verified: " + actualMessage);

            await ClickAsync(btnClose, "Click Okay");
            Console.WriteLine("Close Popop: After Review.");
        }

        // ============================================================
        //  Short-wait probe for the "Seems like you have previously
        //  reviewed" message. Returns true if the popup shows, false on
        //  timeout — matching Selenium's try/catch-returns-false shape.
        //  Preserves Selenium's 15-second waiting window.
        // ============================================================
        public async Task<bool> IsDuplicateReviewPopupVisibleAsync()
        {
            try
            {
                await orderReviewUpdate.WaitForAsync(new LocatorWaitForOptions
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 15000
                });
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
