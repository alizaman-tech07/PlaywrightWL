using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Pages;
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
            var roomServicePage = new RoomServicePage(Page);

            await roomServicePage.OpenRoomServiceAsync();
        }
    }
}
