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
    public class CheckedInHandler :
         IHandleMessages<CheckedIn>
    {
        public Task Handle(CheckedIn message, IMessageHandlerContext context)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<BookingsHub>();

            hubContext.Clients.Client(message.ClientId)
                .checkedIn(new
                {
                    message.BookingNumber,
                });
            return Task.CompletedTask;
        }
    }
}