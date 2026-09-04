using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Pages;
using PlaywrightTest2.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTest2.Test
{
    [TestFixture]
    public class RoomServiceTests : BaseTest
    {
        [Test]
        public async Task RoomServiceTest()
        {
            var alertHelper=new AlertHelper(Page);
            var roomServicePage = new RoomServicePage(Page);
            var checkoutPage=new CheckoutPage(Page);

            await alertHelper.CookiesAlertMethod();
            await roomServicePage.OpenRoomServiceAsync();

            await checkoutPage.RoomServiceAsync("Ali", "ali@yopmail.com", "R-021");

            await Assertions.Expect(checkoutPage.successMessageRoom).ToContainTextAsync("Thank you for your order");


            string orderId = await checkoutPage.GetOrderIdAsync();

            Console.WriteLine("Order ID: " + orderId);

            Assert.That(orderId, Is.Not.Empty, "Order ID was not generated");
            Console.WriteLine("Verified: Thank you for your order");
            Console.WriteLine("Assert : Order Received!");
        }
    }
}
