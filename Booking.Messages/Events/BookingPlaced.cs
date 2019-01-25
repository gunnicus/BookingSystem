namespace Booking.Messages.Events
{
    using Booking.Data;
    using NServiceBus;

    public class BookingPlaced :
        IEvent
    {
        public int BookingNumber { get; set; }
        public BookingItem BookingItem { get; set; }
        public string ClientId { get; set; }
    }
}