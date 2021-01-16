using System;
using System.Collections;
using System.Collections.Generic;

namespace SimControls.Model.AirportDatabase
{
    public interface IAirportRepository
    {
        public IAsyncEnumerable<Airport> SearchAirports(string searchString);
    }

    public class Airport
    {
        public int AirportId { get; set; }
        public string Ident { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public double LonX { get; set; }
        public double LatY { get; set; }
        public int Altitude { get; set; }

        public List<Runway> Runways { get; set; } = new List<Runway>();
        public override string ToString() => $"{Name} ({Ident}) {City}, {State}";

        public string WorldPosition =>
            new Location(LongLat.FromLatitude(LatY), LongLat.FromLongitudde(LonX), Altitude).WorldPosition;
    }

    public class Runway
    {
        public int RunwayId { get; set; }
        public string Name { get; set; } = "";
        public string ReverseName { get; set; } = "";
        public string Surface { get; set; } = "";
        public double Heading => GeoOperations.InitialBearing(LonX, LatY, ReverseLonX, ReverseLatY);
        public double LonX { get; set; }
        public double LatY { get; set; }
        public double ReverseLonX { get; set; }
        public double ReverseLatY { get; set; }
        public Airport Airport { get; set; } = null!;

        public double LengthInFeet => GeoOperations.FeetPerMeter *
                                      GeoOperations.DistanceInMeters(LonX, LatY, ReverseLonX, ReverseLatY);

        public Runway ReverseRunway => new Runway()
        {
            Name = ReverseName,
            ReverseName = Name,
            Surface = Surface,
            LonX = ReverseLonX,
            LatY = ReverseLatY,
            ReverseLonX = LonX,
            ReverseLatY = LatY
        };
    }
}