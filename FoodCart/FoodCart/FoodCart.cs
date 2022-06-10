using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FoodCart
{
    public class FoodCart
    {

        public static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("CMD Line Args Error");
                return;
            }
            
            var numberOfWorkers = Convert.ToInt32(args[0]);
            var timeBetweenCustomers = Convert.ToDouble(args[1]);
            var averageServiceTime = Convert.ToInt32(args[2]);

            //Console.WriteLine($"{numberOfWorkers} {timeBetweenCustomers} {averageServiceTime}");

            var foodCart = new FoodieCart(numberOfWorkers, timeBetweenCustomers, averageServiceTime); ;

            foodCart.Run();
        }
    }
}