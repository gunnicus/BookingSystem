using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Messages.Events
{
    public class CheckedOut :
    IEvent
    {
        public int BookingNumber { get; set; }
        public string ClientId { get; set; }
    }
}
