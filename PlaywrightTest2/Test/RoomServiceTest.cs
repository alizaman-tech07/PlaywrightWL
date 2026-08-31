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

            await alertHelper.CookiesAlertMethod();
            await roomServicePage.OpenRoomServiceAsync();
        }
    }
}
