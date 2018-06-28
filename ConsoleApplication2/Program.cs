using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            _validate(args);
            var origin = args[2];
            var destination = args[4];

            var providerRoutes = FlightDataProvider.GetRouteInformation();

            var requiredRoutes = providerRoutes.Where(r => r.Origin.Equals(origin) && r.Destination.Equals(destination)).ToList();
            if (requiredRoutes == null || requiredRoutes.Count == 0) Console.WriteLine("No Flights found for this route");
            else
                foreach (var route in requiredRoutes)
                    Console.WriteLine($"{route.Origin} --> {route.Destination} ({route.DepartureTime} --> {route.DestinationTime}) - ${route.Rate.Value}");

        }

        private static void _validate(string[] args)
        {
            if (!args[0].Equals("$searchFlights"))
            {
                Console.WriteLine("Something doesnt seem right");
                Environment.Exit(0);
            }
            if (args.Length != 5)
            {
                Console.WriteLine("Args length doesn't match the expected Argument length");
                Environment.Exit(0);
            }
        }
    }
    public class FlightRoute
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime DestinationTime { get; set; }
        public Amount Rate { get; set; }
    }

    public class Amount
    {
        public string Currency { get; set; }
        public decimal Value { get; set; }
    }
}
