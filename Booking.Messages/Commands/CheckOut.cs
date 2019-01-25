using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Messages.Commands
{
    public class CheckOut:
         ICommand
    {
        public int BookingNumber { get; set; }
        public string ClientId { get; set; }
    }
}
