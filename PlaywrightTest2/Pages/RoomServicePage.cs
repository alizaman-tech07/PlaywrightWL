using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTest2.Pages
{
    public class RoomServicePage : BasePage
    {
        
        private readonly ILocator roomService;
        private readonly ILocator menuCategory;

        private readonly ILocator itemsModelPopup;
        private readonly ILocator selectMenuItems;
        private readonly ILocator itemsAddToCart;
        private readonly ILocator cartToCheckout;


        public RoomServicePage(IPage page) : base(page)
        {
            roomService = page.GetByText("Room Service", new() { Exact = false }).First;
            menuCategory = page.Locator("//*[@id='ulMenuCategories']/a[1]");
            itemsModelPopup = page.Locator("//*[@id='dvMenu']/section[1]/div[2]/div[1]");
            selectMenuItems = page.Locator("//*[@id='dvMetaItems']/div[1]");
            itemsAddToCart = page.Locator("//*[contains(text(),'Add for')]");
            cartToCheckout = page.Locator(".basket-checkout-btn-wrap");
        }

        //public async Task OpenAsync()
        //{
        //    await NavigateAsync("https://h10.mysuperbutler.com/");
        //}

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



    }
}
