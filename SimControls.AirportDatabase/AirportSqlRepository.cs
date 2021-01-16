using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        private static IQueryable<Airport> Query(string searchString, AirportDbContext db) =>
            db.Airports.FromSqlRaw("select * from Airport where ((Ident like {1} collate NOCASE) OR (Name like {0} collate NOCASE) or (City like {0} collate NOCASE))", $"%{searchString}%", searchString+"%")
                .Include(i => i.Runways)
                .Take(100);
    }
}