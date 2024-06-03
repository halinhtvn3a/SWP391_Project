using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace DAOs.Helper
{
    public class LocationService
    {
        private const double EarthRadiusInKilometers = 6371.0;
        public static double CalculateDistance(LocationModel point1, LocationModel point2)
        {
            double lat1 = DegreesToRadians(point1.Latitude);
            double lon1 = DegreesToRadians(point1.Longitude);
            double lat2 = DegreesToRadians(point2.Latitude);
            double lon2 = DegreesToRadians(point2.Longitude);

            double latDiff = lat2 - lat1;
            double lonDiff = lon2 - lon1;

            double a = Math.Pow(Math.Sin(latDiff / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin(lonDiff / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusInKilometers * c;
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }
}
