namespace Booking.Messages.Events
{
    using NServiceBus;

    public class BookingCancelled :
        IEvent
    {
        public int BookingNumber { get; set; }
        public string ClientId { get; set; }
    }
}