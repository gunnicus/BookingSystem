using Booking.Messages.Events;
using Microsoft.AspNet.SignalR;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BookingPortal.Handlers
{
    public class CheckedOutHandler :
         IHandleMessages<CheckedOut>
    {
        public Task Handle(CheckedOut message, IMessageHandlerContext context)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BookingsHub>();

            hubContext.Clients.Client(message.ClientId)
                .checkedOut(new
                {
                    message.BookingNumber,
                });
            return Task.CompletedTask;
        }
    }
}