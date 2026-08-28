using Microsoft.Playwright;
using PlaywrightTest2.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightTest2.Pages
{
    public class RoomServicePage : BasePage
    {
        private readonly ILocator roomService;

        public RoomServicePage(IPage page) : base(page)
        {
            roomService = page.GetByText("Room Service", new() { Exact = false }).First;
        }

        public async Task OpenAsync()
        {
            await NavigateAsync("https://h10.mysuperbutler.com/");
        }

        public async Task OpenRoomServiceAsync()
        {
            await roomService.ClickAsync();
        }
    }
}
