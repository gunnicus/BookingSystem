using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Booking.Messages.Commands;
using Booking.Messages.Events;

public class SubmitBookingHandler :
    IHandleMessages<SubmitBooking>
{
    static ILog log = LogManager.GetLogger<SubmitBookingHandler>();

    public Task Handle(SubmitBooking message, IMessageHandlerContext context)
    {
        if (DebugFlagMutator.Debug)
        {
            Debugger.Break();
        }
        
        log.Info("The credit card values will be encrypted when looking at the messages in the queues");
        log.Info($"CreditCard Number is {message.CreditCardNumber}");
        log.Info($"CreditCard Expiration Date is {message.ExpirationDate}");

        // tell the client the order was received
        var orderPlaced = new BookingPlaced
        {
            ClientId = message.ClientId,
            OrderNumber = message.BookingNumber,
            BookingItem = message.BookingItem
        };
        return context.Publish(orderPlaced);
    }
}
