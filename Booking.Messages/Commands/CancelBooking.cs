namespace Booking.Messages.Commands
{
    using NServiceBus;

    public class CancelBooking :
        ICommand
    {
        public int BookingNumber { get; set; }
        public string ClientId { get; set; }
    }
}