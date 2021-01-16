using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SimControls.Model.AirportDatabase;

namespace SimControls.AirportDatabase
{
    public class AirportDbContext: DbContext
    {
        public AirportDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Airport> Airports { get; set; } = null!;
        public DbSet<Runway> Runways { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Airport>().ToTable("Airport");
            mb.Entity<Runway>().ToTable("Runway");
            mb.Entity<Airport>().HasKey(i => i.AirportId);
            mb.Entity<Airport>().HasMany(i => i.Runways).WithOne(i => i.Airport).IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            mb.Entity<Runway>().HasKey(i => i.RunwayId);
        }
        
        
        public static Func<AirportDbContext> DataContextFactory(string fileName)
        {
            var connection = new SqliteConnection("DataSource=" + fileName);
            connection.Open();
            var options = new DbContextOptionsBuilder<AirportDbContext>()
                .UseSqlite(connection).Options;
            // options must be a local variable -- we capture the options and reuse it in th e lambda
            return () => new AirportDbContext(options);
        }

    }
}