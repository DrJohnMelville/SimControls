using System.Collections.Generic;
using System.Linq;
using Moq;
using SimControls.FlightDisplays;
using SimControls.Model.AirportDatabase;
using Xunit;

namespace SimControls.Test.FlightDiisplays
{
    public class FlightPlanViewModelTest
    {
        private readonly List<Airport> db = new()
        {
            new Airport
            {
                Ident = "KCHS", Altitude = 32, City = "CHARLESTON", Name = "CHARLESTON AFB INTERNATIONAL", State = "SC",
                LonX = -80.040, LatY = 32.889
            },
            new Airport
            {
            Ident = "KSEA", Altitude = 32, City = "CHARLESTON", Name = "CHARLESTON AFB INTERNATIONAL", State = "SC",
            LonX = -80.040, LatY = 32.889
        }
        };

            private readonly Mock<IAirportRepository> dbWrapper = new();
        private readonly FlightPlanViewModel sut;

        public FlightPlanViewModelTest()
        {
            dbWrapper.Setup(i => i.SearchAirports(It.IsAny<string>())).Returns(
                db.ToAsyncEnumerable());
            sut = new FlightPlanViewModel(dbWrapper.Object,
                a=>new AirportViewModel(a));
        }

        [Fact]
        public void OriginViewModel()
        {
            Assert.Null(sut.OriginViewModel);
            sut.Origin = db.First();
            Assert.Equal(db.First(), sut.OriginViewModel!.Airport);
            
        }
        [Fact]
        public void DestinationViewModel()
        {
            Assert.Null(sut.DestinationViewModel);
            sut.Destination = db.First();
            Assert.Equal(db.First(), sut.DestinationViewModel!.Airport);
            
        }


        [Fact]
        public void GenerateFlightPlan()
        {
            sut.Origin = db[0];
            sut.Destination = db[1];
            var fp = sut.FlightPlan();
            Assert.Contains("<ATCWaypoint id=\"KCHS\">", fp);
            Assert.Contains("<ATCWaypoint id=\"KSEA\">", fp);
            Assert.Contains("<ICAOIdent>KCHS</ICAOIdent>", fp);
            Assert.Contains("<ICAOIdent>KSEA</ICAOIdent>", fp);
            Assert.Contains("<WorldPosition>N32° 53' 20\",W80° 2' 24\",+32</WorldPosition>", fp);
        }

    }
}