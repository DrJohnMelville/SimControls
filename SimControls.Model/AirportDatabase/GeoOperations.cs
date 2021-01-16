using System;

namespace SimControls.Model.AirportDatabase
{
    public static class GeoOperations
    {
        private const double RadiansPerDegree = Math.PI / 180.0;
        public const double FeetPerMeter = 100 / (12 * 2.54);
        private const double RadiusOfEarth = 6376500.0;

        public static double DistanceInMeters(double longitude, double latitude, double otherLongitude, double otherLatitude)
        {
            var lambda1 = latitude * (RadiansPerDegree);
            var phi1 = longitude * (RadiansPerDegree);
            var lambda2 = otherLatitude * (RadiansPerDegree);
            var phi2 = otherLongitude * (RadiansPerDegree);
            
            var phiDifference = phi2 - phi1;
            var d3 = Square(Math.Sin((lambda2 - lambda1) / 2.0)) + Math.Cos(lambda1) * Math.Cos(lambda2) * Square(Math.Sin(phiDifference / 2.0));
            return RadiusOfEarth * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }

        private static double Square(double x) => x * x;

        // stolen from: https://www.movable-type.co.uk/scripts/latlong.html
        public static double InitialBearing(double longitude, double latitude,
            double otherLongitude, double otherLatitude)
        {
            var lambda1 = latitude * (RadiansPerDegree);
            var phi1 = longitude * (RadiansPerDegree);
            var lambda2 = otherLatitude * (RadiansPerDegree);
            var phi2 = otherLongitude * (RadiansPerDegree);

            var y = Math.Sin(lambda2-lambda1) * Math.Cos(phi2);
            var x = Math.Cos(phi1)*Math.Sin(phi2) -
                Math.Sin(phi1)*Math.Cos(phi2)*Math.Cos(lambda2-lambda1);
            var θ = Math.Atan2(y, x);
            return (θ/RadiansPerDegree + 360) % 360; // in degrees

        }

        public static (double Longitude, double Latitude) DestinationPoint(
            double longitude, double latitude, double bearing, double metersDistance )
        {
            var lambda1 = latitude * (RadiansPerDegree);
            var phi1 = longitude * (RadiansPerDegree);
            var anglarDist = metersDistance/RadiusOfEarth;
            
            var phi2 = Math.Asin( Math.Sin(phi1)*Math.Cos(anglarDist) +
                                  Math.Cos(phi1)*Math.Sin(anglarDist)*
                                  Math.Cos(bearing*RadiansPerDegree) );
            
            var lambda2 = lambda1 + Math.Atan2(Math.Sin(bearing*RadiansPerDegree)*
                                 Math.Sin(anglarDist)*Math.Cos(phi1),
                Math.Cos(anglarDist)-Math.Sin(phi1)*Math.Sin(phi2));
            return ((phi2 / RadiansPerDegree + 540) % 360 - 180, lambda2/RadiansPerDegree);

        }
    }
}