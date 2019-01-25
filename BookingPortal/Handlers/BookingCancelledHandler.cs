using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using NServiceBus;
using Booking.Messages.Events;

public class BookingCancelledHandler :
    IHandleMessages<BookingCancelled>
{
    public Task Handle(BookingCancelled message, IMessageHandlerContext context)
    {
        var hubContext = GlobalHost.ConnectionManager.GetHubContext<BookingsHub>();

        hubContext.Clients.Client(message.ClientId)
            .bookingCancelled(new
            {
                message.BookingNumber,
            });
        return Task.CompletedTask;
    }
}
