using System.Threading;
using Microsoft.AspNet.SignalR;
using NServiceBus;
using System.Diagnostics;
using System.Threading.Tasks;
using Booking.Messages.Commands;
using Booking.Data;

public class BookingsHub :
    Hub
{
    static int bookingNumber;

    public async Task CancelBooking(int bookingNumber)
    {
        var command = new CancelBooking
        {
            ClientId = Context.ConnectionId,
            BookingNumber = bookingNumber
        };
        
        var sendOptions = new SendOptions();
        await MvcApplication.EndpointInstance.Send(command,sendOptions);
    }

    public async Task CheckIn(int bookingNumber)
    {
        var command = new CheckIn
        {
            ClientId = Context.ConnectionId,
            BookingNumber = bookingNumber
        };
        
        var sendOptions = new SendOptions();
        await MvcApplication.EndpointInstance.Send(command, sendOptions);
    }

    public async Task CheckOut(int bookingNumber)
    {
        var command = new CheckOut
        {
            ClientId = Context.ConnectionId,
            BookingNumber = bookingNumber
        };
        
        var sendOptions = new SendOptions();
        await MvcApplication.EndpointInstance.Send(command, sendOptions);
    }

    public async Task PlaceBooking(BookingItem bookingItem)
    {
        var command = new SubmitBooking
        {
            ClientId = Context.ConnectionId,
            BookingNumber = Interlocked.Increment(ref bookingNumber),
            BookingItem = bookingItem
        };

        var sendOptions = new SendOptions();
        await MvcApplication.EndpointInstance.Send(command, sendOptions);
    }
}