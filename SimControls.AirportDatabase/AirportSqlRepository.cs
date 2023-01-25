using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using SimControls.Model.AirportDatabase;

namespace SimControls.AirportDatabase
{
    public class AirportSqlRepository : IAirportRepository
    {
        private readonly Func<AirportDbContext> contextFactory;

        public AirportSqlRepository(Func<AirportDbContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async IAsyncEnumerable<Airport> SearchAirports(string searchString)
        {
            using var db = contextFactory();
            await foreach (var airport in Query(searchString, db).AsAsyncEnumerable())
            {
                yield return airport;
            }
        }

        private static IQueryable<Airport> Query(string searchString, AirportDbContext db)
        {
            if (IsLatLong(searchString, out var latitude, out var longitude))
            {
                return SearchLatLong(latitude, longitude, db);
            }
            return SearchByNames(searchString, db);
        }

        private static IQueryable<Airport> SearchLatLong(double latitude, double longitude,
            AirportDbContext db)
        {
            return db.Airports.Include(i=>i.Runways).OrderBy(i =>
                    (longitude - i.LonX) * (longitude - i.LonX) + (latitude - i.LatY) * (latitude - i.LatY))
                .Take(20);
        }

        private static Regex latLongFinder = new Regex(@"(-?\d+(?:\.\d+)?)[^\d\.]+(-?\d+(?:\.\d+)?)");
        private static bool IsLatLong(string searchString, out double latitude, out double longitude)
        {
            var match = latLongFinder.Match(searchString);
            if (!match.Success)
            {
                latitude = longitude = 0;
                return false;
            }

            latitude = double.Parse(match.Groups[1].Value);
            longitude = double.Parse(match.Groups[2].Value);
            return true;
        }

        private static IQueryable<Airport> SearchByNames(string searchString, AirportDbContext db)
        {
            return db.Airports
                .FromSqlRaw(
                    "select * from Airport where ((Ident like {1} collate NOCASE) OR (Name like {0} collate NOCASE) or (City like {0} collate NOCASE))",
                    $"%{searchString}%", searchString + "%")
                .Include(i => i.Runways)
                .Take(100);
        }
    }
}