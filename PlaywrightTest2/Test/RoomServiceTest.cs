using System;
using System.Threading.Tasks;
using PlaywrightTest2.Base;
using PlaywrightTest2.Pages;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Test
{
    /*
     * Test = flow only.
     * Page actions and logging live in HomePage / RoomServicePage / CheckoutPage / ReviewPage.
     * BaseTest supplies the Playwright Page and the ExtentReports lifecycle.
     */
    [TestFixture]
    public class RoomServiceTest : BaseTest
    {
        [Test]
        [Category("Smoke")]
        public async Task ROOM_SERVICE_Positive_TestCase_01()
        {
            try
            {
                Console.WriteLine("*****************");

                // STAGING — row 2 of each sheet (LIVE uses row 3 per Selenium comment)
                string serviceName = ExcelHelper.GetCell("RoomService", 2, 2);

                string name = ExcelHelper.GetCell("SheetCheckout", 2, 1);
                string email = ExcelHelper.GetCell("SheetCheckout", 2, 2);
                string mobile = ExcelHelper.GetCell("SheetCheckout", 2, 3);
                string room = ExcelHelper.GetCell("SheetCheckout", 2, 4);
                string review = ExcelHelper.GetCell("SheetCheckout", 2, 5);

                string qrPin = ExcelHelper.GetCell("QRskip", 2, 1);

                /* Arrange - START */
                var homePage = new HomePage(Page);
                var roomService = new RoomServicePage(Page);
                var checkout = new CheckoutPage(Page);
                var reviewPage = new ReviewPage(Page);
                /* Arrange - END */

                if (await homePage.IsQRAvailableAsync())
                {
                    await homePage.ClickImageSevenTimesAsync();
                    await homePage.FillQRpinAsync(qrPin);
                }
                else
                {
                    TestContext.Out.WriteLine("QR is OFF. Continuing order flow.");
                }
                await WaitHelper.WaitForDelayAsync(5000);
                await homePage.HomePageServiceAsync();
                await homePage.SelectCategoryAndServiceAsync(
                    Page.Locator("//div[@class='container']//h4[text()='Room Service']"),
                    serviceName);
                Console.WriteLine("The Service is! " + serviceName);
                await roomService.RoomServiceTCAsync();

                // Excel data pass
                await checkout.FillCheckoutDetails_RoomServiceAsync(name, email, mobile, room);

                // Success message
                string actualMessage = await checkout.GetSuccessMessageRoomAsync();
                Assert.That(actualMessage, Does.Contain("Thank you for your order"), "Success message not displayed after order");
                Console.WriteLine("Verified: " + actualMessage);
                Console.WriteLine("Assert : Order Received!");

                Console.WriteLine("Click For Order Review.");
                await reviewPage.SuccessOrderReviewAsync();

                await reviewPage.HandleReviewSubmissionAsync(review);
                Console.WriteLine("Review Test Passed Successfully!");
            }
            catch (Exception ex)
            {
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }

        //[Test]
        //[Category("Express")]
        public async Task ROOM_SERVICE_Express_TestCase_01()
        {
            try
            {
                string serviceName = ExcelHelper.GetCell("RoomService", 2, 2);

                /* Arrange - START */
                var roomService = new RoomServicePage(Page);
                var checkout = new CheckoutPage(Page);
                /* Arrange - END */

               // await roomService.HomePageServiceAsync();
                await roomService.SelectServiceFromListAsync(serviceName);
                Console.WriteLine("The Service is!" + serviceName);
                await roomService.RoomServiceTCAsync();

                // Success message
                string actualMessage = await checkout.GetSuccessMessageRoomAsync();
                Assert.That(actualMessage, Does.Contain("Thank you for your order"), "Success message not displayed after order");
                Console.WriteLine("Thank You!");
            }
            catch (Exception ex)
            {
                Assert.Fail("Test failed due to exception: " + ex.Message);
            }
        }
    }
}
