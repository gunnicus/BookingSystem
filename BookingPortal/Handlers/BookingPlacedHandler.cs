using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NServiceBus;
using Booking.Messages.Events;

public class BookingPlacedHandler :
    IHandleMessages<BookingPlaced>
{
    public Task Handle(BookingPlaced message, IMessageHandlerContext context)
    {
        var hubContext = GlobalHost.ConnectionManager.GetHubContext<BookingsHub>();
        hubContext.Clients.Client(message.ClientId)
            .bookingReceived(new
            {
                message.BookingNumber,
                message.BookingItem
            });
        return Task.CompletedTask;
    }
}