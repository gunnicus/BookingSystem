namespace Booking.Messages.Commands
{
    using Booking.Data;
    using NServiceBus;
    using NServiceBus.Encryption.MessageProperty;

    public class SubmitBooking :
        ICommand
    {
        public int BookingNumber { get; set; }
        public BookingItem BookingItem { get; set; }
        public string ClientId { get; set; }
    }
}