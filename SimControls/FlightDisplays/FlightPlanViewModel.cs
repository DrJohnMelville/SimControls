using System;
using System.Xml;
using System.Xml.Linq;
using Melville.MVVM.BusinessObjects;
using Melville.MVVM.Wpf.DiParameterSources;
using SimControls.Model.AirportDatabase;
using SimControls.SimulatorConnection;

namespace SimControls.FlightDisplays
{
    public class FlightPlanViewModel: NotifyBase
    {
         public IAirportRepository Airports { get; }
         private Func<Airport, AirportViewModel> airportViewModelFactory;

         public FlightPlanViewModel(IAirportRepository airports, 
             Func<Airport, AirportViewModel> airportViewModelFactory)
         {
             Airports = airports;
             this.airportViewModelFactory = airportViewModelFactory;
         }

         private Airport? origin;
         public Airport? Origin
         {
             get => origin;
             set
             {
                 AssignAndNotify(ref origin, value);
                 OriginViewModel = CreateAirportViewModel(value);
             }
         }

         private AirportViewModel? originViewModel;
         public AirportViewModel? OriginViewModel
         {
             get => originViewModel;
             set => AssignAndNotify(ref originViewModel, value);
         }

         private Airport? destination;
         public Airport? Destination
         {
             get => destination;
             set
             {
                 AssignAndNotify(ref destination, value);
                 DestinationViewModel = CreateAirportViewModel(value);
             }
         }

         private AirportViewModel? CreateAirportViewModel(Airport? value) => 
             value == null?null:airportViewModelFactory(value);

         private AirportViewModel? destinationViewModel;
         public AirportViewModel? DestinationViewModel
         {
             get => destinationViewModel;
             set => AssignAndNotify(ref destinationViewModel, value);
         }

        public void SetFlightPlan([FromServices] ISimulatorInterface sim)
         {
             var fp = FlightPlan();
             if (fp == null) return;
             sim.LoadFlightPlan(fp);
         }

         public string? FlightPlan()
         {
             if (Origin == null || Destination == null) return null;
             var FPElement = new XElement("SimBase.Document",
                 new XAttribute("Type", "AceXml"), new XAttribute("version", "1,0"),
                 new XElement("Descr", "AceXML Document"),
                 new XElement("FlightPlan.FlightPlan",
                     DescrElement("Title"),
                     new XElement("FPType", "VFR"),
                     new XElement("CruisingAlt", "5000.000"),
                     new XElement("DepartureID", Origin.Ident),
                     new XElement("DepartureLLA", Origin.WorldPosition),
                     new XElement("DestinationId", Destination.Ident),
                     new XElement("DestinationLLA", Destination.WorldPosition),
                     DescrElement("Descr"),
                     new XElement("DepartureName", Origin.Name),
                     new XElement("DestinationName", Destination.Name),
                     new XElement("AppVersion", 
                         new XElement("AppVersionMajor", "11"),
                         new XElement("AppVersionBuild", "282174")),
                     Waypoint("Airport", Origin.WorldPosition, Origin.Ident),
                     TryDestinationFinal()!,
                     Waypoint("Airport", Destination.WorldPosition, Destination.Ident)
                 ));
             return FPElement.ToString();
         }

         private XElement? TryDestinationFinal()
         {
             var finalRunway = DestinationViewModel?.SelectedRunway;
             if (finalRunway == null) return null;
             return Waypoint("User", FinalWaypoint(finalRunway), null);
         }

         private string FinalWaypoint(Runway finalRunway)
         {
             var (lonX, latY) = GeoOperations.DestinationPoint(finalRunway.LonX, finalRunway.LatY,
                 InverseFinalBearing(finalRunway), 5556);
             return new Location(LongLat.FromLatitude(latY),
                 LongLat.FromLongitudde(lonX),
                 (Destination?.Altitude ?? 0) + 1000).WorldPosition;
         }

         private static double InverseFinalBearing(Runway finalRunway)
         {
             return (finalRunway.Heading + 180.0) % 360.0;
         }

         private XElement Waypoint(string type, string worldPosition, string? ident) =>
             new("ATCWaypoint", new XAttribute("id", ident ?? "POI"),
                 new XElement("ATCWaypointType", type),
                 new XElement("WorldPosition", worldPosition),
                 new XElement("SpeedMaxFP", "-1"),
                 ident == null?null!:new XElement("ICAO", new XElement("ICAOIdent", ident))
             );

         private XElement DescrElement(string descr) => 
             new(descr, $"{Origin?.Name??"Origin"} to {Destination?.Name??"Destination"}");
    }
}