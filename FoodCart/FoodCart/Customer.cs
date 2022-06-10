using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FoodCart
{
    public class Customer
    {
        public DateTime ArrivalTime { get; set; }
        public int CustomerNumber { get; set; }
        public Thread Thread { get; set; }
    }
}
