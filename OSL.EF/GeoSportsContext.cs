using GeoSports.Common.model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace GeoSports.EF
{
    public class GeoSportsContext : DbContext
    {
        IConfigurationRoot _Configuration;

        public GeoSportsContext()
        {
            _Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:Default", "Data Source=geosports.db" }
                }).Build();
        }

        public GeoSportsContext(IConfigurationRoot configuration)
        {
            _Configuration = configuration;
        }

        public DbSet<AthleteEntity> Athletes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = _Configuration["ConnectionStrings:Default"];
            options.UseSqlite(connectionString);
            options.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AthleteConfiguration());
        }
    }
}