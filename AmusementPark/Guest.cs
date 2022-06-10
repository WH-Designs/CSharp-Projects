using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmusementPark
{
    public class Guest
    {
        public DateTime ArrivalTime { get; set; }
        public int GuestNumber { get; set; }
        public Thread Thread { get; set; }
    }
}
