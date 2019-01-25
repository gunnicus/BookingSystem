using System;
using System.Dynamic;
using System.Threading.Tasks;
using Booking.Data;
using Booking.Messages.Commands;
using Booking.Messages.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NServiceBus.Testing;

namespace Booking.Test
{
    [TestClass]
    public class BookingSystemTest
    {
        [TestMethod]
        public async Task SubmitBookingTest()
        {
            var saga = new ProcessBookingSaga()
            {
                Data = new ProcessBookingSaga.BookingData()
            };
            var context = new TestableMessageHandlerContext();

            SubmitBooking submitBooking = new SubmitBooking();
            submitBooking.BookingItem = new BookingItem();
            submitBooking.BookingItem.Name = "Jon snow";
            submitBooking.BookingItem.Phone = "13324124";
            submitBooking.BookingItem.Email = "Jon.Snow@nightswatch.com";
            submitBooking.BookingItem.CheckInDatetime = DateTime.Now;
            submitBooking.BookingItem.CheckOutDatetime = DateTime.Now;

            await saga.Handle(submitBooking, context)
                            .ConfigureAwait(false);

            var bookingPlaced = (BookingPlaced)context.PublishedMessages[0].Message;
            Assert.AreEqual(Constants.Booked, bookingPlaced.BookingItem.Status);
            Assert.AreEqual(Constants.Booked, saga.Data.BookingItem.Status);
        }

        [TestMethod]
        public async Task CancelBookingTest()
        {
            BookingItem bookingItem = CreateBookingItem();
            var saga = new ProcessBookingSaga()
            {
                Data = new ProcessBookingSaga.BookingData() { BookingNumber = 0, ClientId = "JonSnowClientId", BookingItem = bookingItem }
            };
            var context = new TestableMessageHandlerContext();

            CancelBooking cancelBooking = new CancelBooking();
            cancelBooking.BookingNumber = 0;
            cancelBooking.ClientId = "JonSnowClientId";

            await saga.Handle(cancelBooking, context)
                            .ConfigureAwait(false);

            var bookingCanceled = (BookingCancelled)context.PublishedMessages[0].Message;
            Assert.AreEqual(0, bookingCanceled.BookingNumber);
            Assert.AreEqual(Constants.Cancelled, saga.Data.BookingItem.Status);
        }

        [TestMethod]
        public async Task CannotCancelCheckedOutBookingTest()
        {
            BookingItem bookingItem = CreateBookingItem();
            bookingItem.Status = Constants.CheckedOut;

            var saga = new ProcessBookingSaga()
            {
                Data = new ProcessBookingSaga.BookingData() { BookingNumber = 0, ClientId = "JonSnowClientId", BookingItem = bookingItem }
            };
            var context = new TestableMessageHandlerContext();

            CancelBooking cancelBooking = new CancelBooking();
            cancelBooking.BookingNumber = 0;
            cancelBooking.ClientId = "JonSnowClientId";

            await saga.Handle(cancelBooking, context)
                            .ConfigureAwait(false);
            
            Assert.AreEqual(0, context.PublishedMessages.Length);
        }

        [TestMethod]
        public async Task CheckInTest()
        {
            BookingItem bookingItem = CreateBookingItem();

            var saga = new ProcessBookingSaga()
            {
                Data = new ProcessBookingSaga.BookingData() { BookingNumber = 0, ClientId = "JonSnowClientId", BookingItem = bookingItem }
            };
            var context = new TestableMessageHandlerContext();

            CheckIn checkIn = new CheckIn();
            checkIn.BookingNumber = 0;
            checkIn.ClientId = "JonSnowClientId";

            await saga.Handle(checkIn, context)
                            .ConfigureAwait(false);

            var checkedInEvent = (CheckedIn)context.PublishedMessages[0].Message;
            Assert.AreEqual(0, checkedInEvent.BookingNumber);
            Assert.AreEqual(Constants.CheckedIn, saga.Data.BookingItem.Status);
        }

        [TestMethod]
        public async Task CheckOutTest()
        {
            BookingItem bookingItem = CreateBookingItem();
            bookingItem.Status = Constants.CheckedIn;

            var saga = new ProcessBookingSaga()
            {
                Data = new ProcessBookingSaga.BookingData() { BookingNumber = 0, ClientId = "JonSnowClientId", BookingItem = bookingItem }
            };
            var context = new TestableMessageHandlerContext();

            CheckOut checkOut = new CheckOut();
            checkOut.BookingNumber = 0;
            checkOut.ClientId = "JonSnowClientId";

            await saga.Handle(checkOut, context)
                            .ConfigureAwait(false);

            var checkedOutEvent = (CheckedOut)context.PublishedMessages[0].Message;
            Assert.AreEqual(0, checkedOutEvent.BookingNumber);
            Assert.AreEqual(Constants.CheckedOut, saga.Data.BookingItem.Status);
        }

        [TestMethod]
        public async Task CannotCheckOutTest()
        {
            BookingItem bookingItem = CreateBookingItem();
            bookingItem.Status = Constants.Booked;

            var saga = new ProcessBookingSaga()
            {
                Data = new ProcessBookingSaga.BookingData() { BookingNumber = 0, ClientId = "JonSnowClientId", BookingItem = bookingItem }
            };
            var context = new TestableMessageHandlerContext();

            CheckOut checkOut = new CheckOut();
            checkOut.BookingNumber = 0;
            checkOut.ClientId = "JonSnowClientId";

            await saga.Handle(checkOut, context)
                            .ConfigureAwait(false);

            Assert.AreEqual(0, context.PublishedMessages.Length);
        }

        private BookingItem CreateBookingItem()
        {
            BookingItem bookingItem = new BookingItem();
            bookingItem.Name = "Jon snow";
            bookingItem.Phone = "13324124";
            bookingItem.Email = "Jon.Snow@nightswatch.com";
            bookingItem.CheckInDatetime = DateTime.Now;
            bookingItem.CheckOutDatetime = DateTime.Now;
            bookingItem.Status = Constants.Booked;

            return bookingItem;
        }
    }
}
