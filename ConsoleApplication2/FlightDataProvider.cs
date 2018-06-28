using FlightEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightEngine
{
    public class FlightDataProvider
    {

        internal static List<FlightRoute> GetRouteInformation()
        {
            var routes = new List<FlightRoute>();
            var provider1Data = new FlightDataProvider().GetFlightRouteFromFile("provider1.txt", ',', "M/d/yyyy H:mm:ss");
            var provider2Data = new FlightDataProvider().GetFlightRouteFromFile("provider2.txt", ',', "M-d-yyyy H:mm:ss");
            var provider3Data = new FlightDataProvider().GetFlightRouteFromFile("provider3.txt", '|', "M/d/yyyy H:mm:ss");

            routes.AddRange(provider1Data);
            routes.AddRange(provider2Data);
            routes.AddRange(provider3Data);

            routes = GetDistinctRoutesWithLowestFare(routes).OrderBy(rR => rR.Rate.Value).ThenBy(rR => rR.DepartureTime).ToList();

            return routes;
        }

        private static List<FlightRoute> GetDistinctRoutesWithLowestFare(List<FlightRoute> routes)
        {
            var distinctRoutes = new List<FlightRoute>();
            routes.ForEach(r1 =>
            {
                var matchingRoute = distinctRoutes.FirstOrDefault(r2 => r1.Origin == r2.Origin && r1.DepartureTime == r2.DepartureTime && r1.Destination == r2.Destination && r1.DestinationTime == r2.DestinationTime);
                if (matchingRoute == null)
                {
                    distinctRoutes.Add(r1);
                    return;
                }
                if (matchingRoute.Rate.Value > r1.Rate.Value) //Assuming the currency is same always that is USD
                    matchingRoute.Rate.Value = r1.Rate.Value;

            });

            return distinctRoutes;
        }

        private List<FlightRoute> GetFlightRouteFromFile(string path, char delimiter, string dateTimeFormat)
        {
            var dataRows = File.ReadAllLines(path).ToList();
            var flightRoutes = new List<FlightRoute>();
            foreach (var row in dataRows)
            {
                var columns = row.Split(new char[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                if (columns.Count() != 5)
                {
                    continue;
                }
                
                var route = ParseFlightRoute(columns, dateTimeFormat);
                if(route != null)
                    flightRoutes.Add(route);
            }

            if(flightRoutes.Count == 0)
                Console.WriteLine("Parsing exception");

            return flightRoutes;
        }

        private FlightRoute ParseFlightRoute(string[] columns, string dateTimeFormat)
        {
            try
            {
                return new FlightRoute
                {
                    Origin = columns.ElementAt(0),
                    DepartureTime = DateTime.ParseExact(columns.ElementAt(1), dateTimeFormat, CultureInfo.InvariantCulture),
                    Destination = columns.ElementAt(2),
                    DestinationTime = DateTime.ParseExact(columns.ElementAt(3), dateTimeFormat, CultureInfo.InvariantCulture),
                    Rate = new Amount
                    {
                        Value = decimal.Parse(Regex.Match(columns.ElementAt(4), @"\d+").Value),
                        Currency = "USD" //Need to properly pass the currency
                    }
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
