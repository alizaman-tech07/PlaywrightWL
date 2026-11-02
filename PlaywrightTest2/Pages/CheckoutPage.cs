using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using PlaywrightTest2.Base;
using PlaywrightTest2.Utilities;

namespace PlaywrightTest2.Pages
{
    public class CheckoutPage : BasePage
    {
        // ============================================================
        //  EXISTING LOCATORS — kept exactly as before (public contract)
        // ============================================================
        private readonly ILocator successMessageBooking;
        public ILocator successMessageRoom;
        private readonly ILocator orderText;
        private readonly ILocator userName;
        private readonly ILocator userEmail;
        private readonly ILocator roomNo;
        private readonly ILocator placeOrderBtn;

        // ============================================================
        //  ADDED LOCATORS — ported 1:1 from Selenium CheckoutPage.cs
        // ============================================================
        // Review
        private readonly ILocator successReviewHotel;
        private readonly ILocator orderReview;
        private readonly ILocator textReview;
        private readonly ILocator btnReviewSubmit;
        private readonly ILocator orderReviewUpdate;
        private readonly ILocator btnCancel;
        private readonly ILocator btnUpdate;
        private readonly ILocator btnOkay;

        // Checkout form fields
        private readonly ILocator dateTime;
        private readonly ILocator tableNo;
        private readonly ILocator mobileNo;

        // Buttons
        private readonly ILocator paymentMethod;
        private readonly ILocator placeRequestBtn;
        private readonly ILocator bookNowBtn;

        // Laundry — collection
        private readonly ILocator collectionDay;
        private readonly ILocator collectionDaySelect;
        private readonly ILocator collectionTime;
        private readonly ILocator collectionTimeSelect;
        private readonly ILocator collectionInstruction;
        private readonly ILocator collectionInstructionSelect;

        // Laundry — delivery
        private readonly ILocator deliveryDay;
        private readonly ILocator deliveryDaySelect;
        private readonly ILocator deliveryTime;
        private readonly ILocator deliveryTimeSelect;
        private readonly ILocator deliveryInstruction;
        private readonly ILocator deliveryInstructionSelect;

        // Room Booking — datepicker
        private readonly ILocator selectCheckout;
        private readonly ILocator nextMonth;
        private readonly ILocator selectDate;

        // Scroll anchor elements
        private readonly ILocator dateTimeScroll;
        private readonly ILocator collectionScroll;
        private readonly ILocator deliveryScroll;
        private readonly ILocator paymentScroll;

        public CheckoutPage(IPage page) : base(page)
        {
            // ----- Existing locators (unchanged) -----
            successMessageBooking = page.Locator("//h2[contains(text(),'Booking Received')]");
            successMessageRoom = page.Locator("//h2[contains(text(),'Thank you for your order')]");
            orderText = page.Locator("//span[contains(@class,'order-id-heading')]");

            userName = page.Locator("//*[@id='dvGuestName']//*[@id='txtName']");
            userEmail = page.Locator("//*[@id='dvGuestEmail']//*[@id='txtGuestEmail']");
            roomNo = page.Locator("//*[@id='dvGuestRoomNo']//*[@id='txtGuestRoomNo']");
            placeOrderBtn = page.Locator("//*[contains(text(),'Place Order')]");

            // ----- Added locators (Selenium parity) -----
            successReviewHotel = page.Locator("//*[@id='enterpriseReviewModal']/div/div/div[4] //strong[contains(text(),'Thank You')]");
            orderReview = page.Locator("//*[@id='ctl00_ContentPlaceHolder1_dvRateYourExperience']");
            textReview = page.Locator("//*[@id='enterpriseReviewModal']/div/div/div[2]/div//textarea");
            btnReviewSubmit = page.Locator("#btnEnterpriseReviewsSave");
            orderReviewUpdate = page.Locator("//*[@id='enterpriseReviewModal']  //div[contains(text(),'Seems like you have previously reviewed')]");
            btnCancel = page.Locator("//*[@id='enterpriseReviewModal']  //button[contains(text(),'Cancel')]");
            btnUpdate = page.Locator("#btnEnterpriseReviewsSave");
            btnOkay = page.Locator("#spClose");

            dateTime = page.Locator("//*[contains(@data-target,'#OrderTimeModal')]");
            tableNo = page.Locator("#txtTableNo");
            mobileNo = page.Locator("#txtGuestNumber");

            paymentMethod = page.Locator("//*[contains(text(),'Pay by Cash')]");
            placeRequestBtn = page.Locator("//*[contains(text(),'Submit Requests')]");
            bookNowBtn = page.Locator("//*[contains(text(),'Book Now')]");

            collectionDay = page.Locator("//*[@id='dvLaundryCollectionTime']/div[1]/div[1]/div[2]");
            collectionDaySelect = page.Locator("//*[@id='dvLaundryCollectionTime']/div[1]/div[1]/div[2] //a[1]");
            collectionTime = page.Locator("//*[@id='dvLaundryCollectionTime']/div[1]/div[2]/div[2]");
            collectionTimeSelect = page.Locator("//*[@id='dvLaundryCollectionTime']/div[1]/div[2]/div[2] //a[2]");
            collectionInstruction = page.Locator("//*[@id='dvLaundryCollectionTime']/div[2]/div[2]");
            collectionInstructionSelect = page.Locator("//*[@id='dvLaundryCollectionTime']/div[2]/div[2] //a[1]");

            deliveryDay = page.Locator("//*[@id='dvLaundryDeliveryTime']/div[1]/div[1]/div[2]");
            deliveryDaySelect = page.Locator("//*[@id='dvLaundryDeliveryTime']/div[1]/div[1]/div[2] //a[1]");
            deliveryTime = page.Locator("//*[@id='dvLaundryDeliveryTime']/div[1]/div[2]/div[2]");
            deliveryTimeSelect = page.Locator("//*[@id='dvLaundryDeliveryTime']/div[1]/div[2]/div[2] //a[5]");
            deliveryInstruction = page.Locator("//*[@id='dvLaundryDeliveryTime']/div[2]/div[2]");
            deliveryInstructionSelect = page.Locator("//*[@id='dvLaundryDeliveryTime']/div[2]/div[2] //a[2]");

            selectCheckout = page.Locator("//*[@id='txtCheck_Out']");
            nextMonth = page.Locator("//body/div[18]/div[1]/div[1]/button[2]");
            selectDate = page.Locator("/ html[1] / body[1] / div[18] / div[1] / div[2] / table[1] / tbody[1] / tr[4] / td[4] / div[1]");

            dateTimeScroll = page.Locator("//*[contains(text(),'Check-In Date & Time')]");
            collectionScroll = page.Locator("//*[contains(text(),'Collection')]");
            deliveryScroll = page.Locator("//*[contains(text(),'Delivery Time')]");
            paymentScroll = page.Locator("//*[contains(text(),'Payment Options')]");
        }

        // ============================================================
        //  EXISTING METHODS — kept exactly as before
        // ============================================================

        // For Order-ID Print
        public async Task<string> GetOrderIdAsync()
        {
            string fullText = await orderText.InnerTextAsync();
            return Regex.Match(fullText, @"\d+").Value;
        }

        // RoomService Checkout (no-logging quick path; preserved verbatim).
        public async Task RoomServiceAsync(string user, string pass, string room)
        {
            await userName.FillAsync(user);
            await userEmail.FillAsync(pass);
            await roomNo.FillAsync(room);
            await placeOrderBtn.ClickAsync();
        }

        // ============================================================
        //  ADDED METHODS — ported from Selenium CheckoutPage.cs
        //  Logging + step screenshots delegated to BasePage wrappers.
        // ============================================================

        public async Task<string> GetSuccessMessageBookingAsync()
        {
            await successMessageBooking.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            return await successMessageBooking.InnerTextAsync();
        }

        public async Task<string> GetSuccessMessageRoomAsync()
        {
            await successMessageRoom.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
            return await successMessageRoom.InnerTextAsync();
        }

        public async Task SuccessOrderReviewAsync()
        {
            await ClickAsync(orderReview, "Click For Order Review");
        }

        // ----- Field entry -----
        public async Task EnterUserNameAsync(string name) =>
            await EnterTextAsync(userName, name, "Enter Full Name");

        public async Task EnterUserEmailAsync(string email) =>
            await EnterTextAsync(userEmail, email, "Enter Email");

        public async Task EnterRoomNoAsync(string roomNumber) =>
            await EnterTextAsync(roomNo, roomNumber, "Enter Room Number");

        public async Task EnterTableNoAsync(string tableNumber) =>
            await EnterTextAsync(tableNo, tableNumber, "Enter Table No");

        public async Task EnterMobileNoAsync(string mobileNumber)
        {
            await EnterTextAsync(mobileNo, mobileNumber, "Enter Mobile N0");
            await WaitHelper.WaitForDelayAsync(3000);
        }

        // ----- Laundry — collection -----
        public async Task SelectCollectionDayClickAsync()
        {
            Console.WriteLine("Click Collection Day");
            await collectionDay.ClickAsync();
            Console.WriteLine("Select Collection Day");
            await collectionDaySelect.ClickAsync();
        }

        public async Task SelectCollectionTimeClickAsync()
        {
            Console.WriteLine("Click Collection Time");
            await collectionTime.ClickAsync();
            Console.WriteLine("Select Collection Time");
            await collectionTimeSelect.ClickAsync();
        }

        public async Task SelectCollection_InstructionClickAsync()
        {
            Console.WriteLine("Click Collection Instruction");
            await collectionInstruction.ClickAsync();
            Console.WriteLine("Select Collection Instruction");
            await collectionInstructionSelect.ClickAsync();
        }

        // ----- Laundry — delivery -----
        public async Task SelectDeliveryDayClickAsync()
        {
            Console.WriteLine("Click Delivery Day");
            await deliveryDay.ClickAsync();
            Console.WriteLine("Select Delivery Day");
            await deliveryDaySelect.ClickAsync();
        }

        public async Task SelectDeliveryTimeClickAsync()
        {
            Console.WriteLine("Click Delivery Time");
            await deliveryTime.ClickAsync();
            Console.WriteLine("Select Delivery Time");
            await deliveryTimeSelect.ClickAsync();
        }

        public async Task SelectDelivery_InstructionClickAsync()
        {
            Console.WriteLine("Click Delivery Instruction");
            await deliveryInstruction.ClickAsync();
            Console.WriteLine("Select Delivery Instruction");
            await deliveryInstructionSelect.ClickAsync();
        }

        // ----- Room Booking datepicker -----
        public async Task SelectCheckoutDateTimeClickAsync()
        {
            Console.WriteLine("Click Check-out Date Time");
            await selectCheckout.ClickAsync();
            await WaitHelper.WaitForDelayAsync(2000);
            Console.WriteLine("Click Next Month Time");
            await nextMonth.ClickAsync();
            await WaitHelper.WaitForDelayAsync(2000);
            Console.WriteLine("Select Date");
            await selectDate.ClickAsync();
        }

        // ----- Common action buttons -----
        public async Task SelectPaymentMethodAsync() =>
            await ClickAsync(paymentMethod, "Select Payment.");

        public async Task ClickPlaceOrderAsync() =>
            await ClickAsync(placeOrderBtn, "Click on Place Order Button");

        public async Task ClickBookNowAsync() =>
            await ClickAsync(bookNowBtn, "Click on BookNow Button");

        public async Task ClickPlaceRequestAsync() =>
            await ClickAsync(placeRequestBtn, "Click on Submit Request Button");

        // ============================================================
        //  CHECKOUT FLOWS (one per service family)
        //  Method names mirror Selenium (typos corrected: RoomServcie -> RoomService,
        //  Reataurant -> Restaurant, Housekeeing -> Housekeeping). Flagged in review.
        // ============================================================

        // Room Service
        public async Task FillCheckoutDetails_RoomServiceAsync(string name, string email, string mobileNumber, string roomNumber)
        {
            Console.WriteLine("*Enter Name");
            await EnterUserNameAsync(name);
            Console.WriteLine("*Enter Email");
            await EnterUserEmailAsync(email);
            Console.WriteLine("*Enter Mobile Number");
            await EnterMobileNoAsync(mobileNumber);
            Console.WriteLine("*Enter Room Number");
            await EnterRoomNoAsync(roomNumber);

            Console.WriteLine("Scroll Down Checkout");
            await ScrollPageAsync(paymentScroll, "Scroll Down For Checkout Datetime : ElementScroll Click");

            await SelectPaymentMethodAsync();
            Console.WriteLine("Select Payment Method");

            await ClickPlaceOrderAsync();
            Console.WriteLine("Place Order Successfully.");

            string orderId = await GetOrderIdAsync();
            Console.WriteLine("Order No is: " + orderId);
            await LogStepAsync("Order No is: " + orderId);

            await WaitHelper.WaitForDelayAsync(500);
        }

        // Restaurant & Cafe
        public async Task FillCheckoutDetails_RestaurantCafeAsync(string name, string email, string mobileNumber, string tableNumber)
        {
            Console.WriteLine("Enter Name");
            await EnterUserNameAsync(name);
            Console.WriteLine("Enter Email");
            await EnterUserEmailAsync(email);
            Console.WriteLine("Enter Mobile Number");
            await EnterMobileNoAsync(mobileNumber);
            Console.WriteLine("Enter Table Number");
            await EnterTableNoAsync(tableNumber);

            Console.WriteLine("Scroll Down Checkout");
            await ScrollPageAsync(paymentScroll, "Scroll Down For Checkout Datetime : ElementScroll Click");

            await SelectPaymentMethodAsync();
            Console.WriteLine("Select Payment Method");

            await ClickPlaceOrderAsync();
            Console.WriteLine("Place Order Successfully.");

            string orderId = await GetOrderIdAsync();
            Console.WriteLine("Order No is: " + orderId);

            await WaitHelper.WaitForDelayAsync(500);
        }

        // Laundry
        public async Task FillLaundry_CheckoutDetailsAsync(string name, string email, string mobileNumber, string roomNumber)
        {
            Console.WriteLine("Enter Name");
            await EnterUserNameAsync(name);
            Console.WriteLine("Enter Email");
            await EnterUserEmailAsync(email);
            Console.WriteLine("Enter Mobile Number");
            await EnterMobileNoAsync(mobileNumber);
            await WaitHelper.WaitForDelayAsync(300);
            Console.WriteLine("Enter Room Number");
            await EnterRoomNoAsync(roomNumber);

            Console.WriteLine("Scroll Down For Collection Checkout");
            await ScrollPageAsync(collectionScroll, "Scroll Down For Collection");

            Console.WriteLine("Select Collection Day");
            await SelectCollectionDayClickAsync();
            Console.WriteLine("Select Collection Time");
            await SelectCollectionTimeClickAsync();
            Console.WriteLine("Select Collection Instruction");
            await SelectCollection_InstructionClickAsync();

            await ScrollPageAsync(deliveryScroll, "Scroll Down For Delivery");

            Console.WriteLine("Select Delivery Day");
            await SelectDeliveryDayClickAsync();
            Console.WriteLine("Select Delivery Time");
            await SelectDeliveryTimeClickAsync();
            Console.WriteLine("Select Delivery Instruction");
            await SelectDelivery_InstructionClickAsync();

            Console.WriteLine("Scroll Down For Delivery");
            await ScrollPageAsync(paymentScroll, "Scroll Down For Payment");

            await SelectPaymentMethodAsync();
            Console.WriteLine("Select Payment Method");

            await ScrollHelper.SmoothScrollDownAsync(Page, 100);

            await ClickPlaceOrderAsync();
            Console.WriteLine("Place Order Successfully.");

            string orderId = await GetOrderIdAsync();
            Console.WriteLine("Order No is: " + orderId);
            await LogStepAsync("Order No is: " + orderId);

            await WaitHelper.WaitForDelayAsync(500);
        }

        // Car Rental | Meeting & Events | Travel & Tour
        public async Task FillCheckoutDetails_BookNowAsync(string name, string email, string mobileNumber, string roomNumber)
        {
            Console.WriteLine("Enter Name");
            await EnterUserNameAsync(name);
            Console.WriteLine("Enter Email");
            await EnterUserEmailAsync(email);
            Console.WriteLine("Enter Mobile Number");
            await EnterMobileNoAsync(mobileNumber);
            Console.WriteLine("Enter Room Number");
            await EnterRoomNoAsync(roomNumber);

            await ScrollPageAsync(paymentScroll, "Scroll Down For Payment.");

            await SelectPaymentMethodAsync();
            Console.WriteLine("Select Payment Method");

            await ScrollHelper.SmoothScrollDownAsync(Page, 300);
            Console.WriteLine("Scroll Down Checkout");

            await ClickBookNowAsync();
            Console.WriteLine("Place Order Successfully.");

            string orderId = await GetOrderIdAsync();
            Console.WriteLine("Order No is: " + orderId);
            await LogStepAsync("Order No is: " + orderId);

            await WaitHelper.WaitForDelayAsync(500);
        }

        // Room Booking
        public async Task FillRoombooking_CheckoutDetailsAsync(string name, string email, string mobileNumber, string roomNumber)
        {
            Console.WriteLine("Enter Name");
            await EnterUserNameAsync(name);
            Console.WriteLine("Enter Email");
            await EnterUserEmailAsync(email);
            Console.WriteLine("Enter Mobile Number");
            await EnterMobileNoAsync(mobileNumber);
            await WaitHelper.WaitForDelayAsync(300);
            Console.WriteLine("Enter Room Number");
            await EnterRoomNoAsync(roomNumber);
            await WaitHelper.WaitForDelayAsync(200);

            Console.WriteLine("Scroll Down For Checkout Datetime");
            await ScrollPageAsync(dateTimeScroll, "Scroll Down For Checkout Datetime : ElementScroll Click");
            await WaitHelper.WaitForDelayAsync(2000);
            Console.WriteLine("Select Checkout Datetime");
            await SelectCheckoutDateTimeClickAsync();
            await WaitHelper.WaitForDelayAsync(3000);

            Console.WriteLine("Scroll Down For Payment Method");
            await ScrollPageAsync(paymentScroll, "Scroll Down For Payment.");

            await SelectPaymentMethodAsync();
            Console.WriteLine("Select Payment Method");

            await ClickPlaceOrderAsync();
            Console.WriteLine("Place Order Successfully.");

            string orderId = await GetOrderIdAsync();
            Console.WriteLine("--- Order No is: " + orderId + " ---");
            await LogStepAsync("Order No is: " + orderId);

            await WaitHelper.WaitForDelayAsync(500);
        }

        // HouseKeeping
        public async Task FillCheckoutDetails_HousekeepingAsync(string name, string email, string roomNumber)
        {
            Console.WriteLine("*Enter Name");
            await EnterUserNameAsync(name);
            Console.WriteLine("*Enter Email");
            await EnterUserEmailAsync(email);
            Console.WriteLine("*Enter Room Number");
            await EnterRoomNoAsync(roomNumber);

            Console.WriteLine("Scroll Down Checkout");

            await ClickPlaceRequestAsync();
            Console.WriteLine("Submit Request Successfully.");

            await WaitHelper.WaitForDelayAsync(500);
        }
    }
}
