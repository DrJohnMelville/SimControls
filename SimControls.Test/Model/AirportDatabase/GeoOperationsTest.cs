using SimControls.Model.AirportDatabase;
using Xunit;

namespace SimControls.Test.Model.AirportDatabase
{
    public class GeoOperationsTest
    {
        private double DMS(double d, double m, double s) => d + (m / 60) + (s / 3600);
        [Fact]
        public void DistanceBetween()
        {
            Assert.Equal(157.37715, GeoOperations.DistanceInMeters(3.999,1,4,0.999), 4);
        }
        [Fact]
        public void Bearing()
        {
            Assert.Equal(315.06988, GeoOperations.InitialBearing(3.999,1,4,0.999), 4);
        }

        [Fact]
        public void DistPoint()
        {
            var (longitude, latitude) = GeoOperations.DestinationPoint(1, 53, 45, 10000);
            Assert.Equal(1.063536, longitude, 4);
            Assert.Equal(53.063547772, latitude, 4);
        }
    
    }
}