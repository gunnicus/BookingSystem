using System;
using System.Diagnostics;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Booking.Messages.Commands;
using Booking.Messages.Events;
using Booking.Data;

public class ProcessBookingSaga :
    Saga<ProcessBookingSaga.BookingData>,
    IAmStartedByMessages<SubmitBooking>,
    IHandleMessages<CancelBooking>,
    IHandleMessages<CheckIn>,
    IHandleMessages<CheckOut>
{
    static ILog log = LogManager.GetLogger<ProcessBookingSaga>();

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<BookingData> mapper)
    {
        mapper.ConfigureMapping<SubmitBooking>(message => message.BookingNumber)
            .ToSaga(sagaData => sagaData.BookingNumber);
        mapper.ConfigureMapping<CancelBooking>(message => message.BookingNumber)
            .ToSaga(sagaData => sagaData.BookingNumber);
        mapper.ConfigureMapping<CheckIn>(message => message.BookingNumber)
           .ToSaga(sagaData => sagaData.BookingNumber);
        mapper.ConfigureMapping<CheckOut>(message => message.BookingNumber)
           .ToSaga(sagaData => sagaData.BookingNumber);
    }

    public Task Handle(SubmitBooking message, IMessageHandlerContext context)
    {
        Data.BookingNumber = message.BookingNumber;
        Data.BookingItem = message.BookingItem;
        Data.ClientId = message.ClientId;

        log.Info($"Received Booking request #{message.BookingNumber} .");

        Data.BookingItem.Status = Constants.Booked;

        // tell the client the order was received
        var bookingPlaced = new BookingPlaced
        {
            ClientId = message.ClientId,
            BookingNumber = message.BookingNumber,
            BookingItem = message.BookingItem
        };
        return context.Publish(bookingPlaced);
    }

    public Task Handle(CancelBooking message, IMessageHandlerContext context)
    {
        //cannot cancel a booking already checked out
        if (Data.BookingItem.Status == Constants.CheckedOut)
        {
            log.Error($"Cannot cancel out booking #{message.BookingNumber} .");
            return Task.CompletedTask;
        }

        Data.BookingItem.Status = Constants.Cancelled;

        MarkAsComplete();

        log.Info($"Booking #{message.BookingNumber} was cancelled.");

        var orderCancelled = new BookingCancelled
        {
            BookingNumber = message.BookingNumber,
            ClientId = message.ClientId
        };
        return context.Publish(orderCancelled);
    }

    public Task Handle(CheckIn message, IMessageHandlerContext context)
    {
        //cannot check out unless it checked in already
        if (Data.BookingItem.Status != Constants.Booked)
        {
            log.Error($"Cannot check in booking #{message.BookingNumber} .");
            return Task.CompletedTask;
        }

        Data.BookingItem.Status = Constants.CheckedIn;

        log.Info($"Booking #{message.BookingNumber} Checked in.");

        var checkedIn = new CheckedIn
        {
            BookingNumber = message.BookingNumber,
            ClientId = message.ClientId
        };
        return context.Publish(checkedIn);
    }

    public Task Handle(CheckOut message, IMessageHandlerContext context)
    {
        //cannot check out unless it checked in already
        if (Data.BookingItem.Status != Constants.CheckedIn)
        {
            log.Error($"Cannot check out booking #{message.BookingNumber} .");
            return Task.CompletedTask;
        }

        Data.BookingItem.Status = Constants.CheckedOut;

        log.Info($"Booking #{message.BookingNumber} Checked out.");

        var checkedIn = new CheckedOut
        {
            BookingNumber = message.BookingNumber,
            ClientId = message.ClientId
        };
        return context.Publish(checkedIn);
    }



    public class BookingData :
        ContainSagaData
    {
        public int BookingNumber { get; set; }
        public BookingItem BookingItem { get; set; }
        public string ClientId { get; set; }
    }
    

}