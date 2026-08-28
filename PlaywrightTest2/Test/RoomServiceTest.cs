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
        private RoomServicePage roomServicePage = null!;

        [SetUp]
        public void PageSetup()
        {
            roomServicePage = new RoomServicePage(Page);
        }

        [Test]
        public async Task Test001()
        {
            await roomServicePage.OpenAsync();

            await roomServicePage.OpenRoomServiceAsync();
        }
    }
}
