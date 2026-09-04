using Microsoft.Playwright;
using PlaywrightTest2.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace PlaywrightTest2.Pages
{
    public class CheckoutPage : BasePage
    {

        private readonly ILocator successMessageBooking;
        public ILocator successMessageRoom;
        private readonly ILocator orderText;
        private readonly ILocator userName;
        private readonly ILocator userEmail;
        private readonly ILocator roomNo;
        private readonly ILocator placeOrderBtn;

        public CheckoutPage(IPage page) : base(page)
        {
            successMessageBooking = page.Locator("//h2[contains(text(),'Booking Received')]");
            successMessageRoom = page.Locator("//h2[contains(text(),'Thank you for your order')]");
            orderText = page.Locator("//span[contains(text(),'Order #')]");

            userName = page.Locator("//*[@id='dvGuestName']//*[@id='txtName']");
            userEmail = page.Locator("//*[@id='dvGuestEmail']//*[@id='txtGuestEmail']");
            roomNo = page.Locator("//*[@id='dvGuestRoomNo']//*[@id='txtGuestRoomNo']");
            placeOrderBtn = page.Locator("//*[contains(text(),'Place Order')]");
            /*
            private By placeOrderBtn = By.XPath("//*[contains(text(),'Place Order')]");
            private By placeRequestBtn = By.XPath("//*[contains(text(),'Submit Requests')]");
            private By bookNowBtn = By.XPath("//*[contains(text(),'Book Now')]");
            */
        }


        // For Order-ID Print
        public async Task<string> GetOrderIdAsync()
        {
            string fullText = await orderText.InnerTextAsync();
            return Regex.Match(fullText, @"\d+").Value;
        }

        // RoomService Checkout
        public async Task RoomServiceAsync(string user, string pass, string room)
        {
            await userName.FillAsync(user);
            await userEmail.FillAsync(pass);
            await roomNo.FillAsync(room);
            await placeOrderBtn.ClickAsync();
        }





    }
}
