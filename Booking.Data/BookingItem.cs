using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Booking.Data
{
    public class BookingItem
    {
        public string Name { get; set; }
        
        public string Phone { get; set; }
        
        public string Email { get; set; }
        
        public DateTime CheckInDatetime { get; set; }

        public DateTime CheckOutDatetime { get; set; }

        public string Status { get; set; }
    }
}