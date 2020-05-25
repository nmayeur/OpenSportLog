/* Copyright 2020 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using OSL.Common.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace OSL.EF
{
    public class OpenSportLogContext : DbContext
    {
        IConfigurationRoot _Configuration;

        private readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

        public OpenSportLogContext()
        {

            _Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>()
            {
                    { "ConnectionStrings:Default", "Data Source=OSL.db" }
            }).Build();
        }

        public OpenSportLogContext(IConfigurationRoot configuration)
        {
            _Configuration = configuration;
        }

        public DbSet<AthleteEntity> Athletes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = _Configuration["ConnectionStrings:Default"];
            options.UseSqlite(connectionString);
            var serviceProvider = new ServiceCollection()
                      .AddLogging(loggingBuilder =>
                      {
                          // configure Logging with NLog
                          loggingBuilder.ClearProviders();
                          loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                          loggingBuilder.AddNLog(_Logger.Factory.Configuration);
                      })
                      .BuildServiceProvider();
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            options.UseLoggerFactory(loggerFactory);
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AthleteEntity>().HasKey(x => x.Id);
            builder.Entity<AthleteEntity>().Property(x => x.Name).IsRequired();
            builder.Entity<AthleteEntity>().HasAlternateKey(x => x.Name); // Unique
            builder.Entity<AthleteEntity>().HasMany<ActivityEntity>(a => a.Activities).WithOne(a => a.Athlete);

            builder.Entity<ActivityEntity>().HasKey("Id");
            builder.Entity<ActivityEntity>().HasMany<TrackEntity>(x => x.Tracks);


            builder.Entity<TrackEntity>().Property<int>("Id");
            builder.Entity<TrackEntity>().HasKey("Id");
            builder.Entity<TrackEntity>().HasMany(t => t.TrackSegments);

            builder.Entity<TrackSegmentEntity>().Property<int>("Id");
            builder.Entity<TrackSegmentEntity>().HasKey("Id");
            builder.Entity<TrackSegmentEntity>().OwnsMany(s => s.TrackPoints, tp =>
                    {
                        tp.Property<int>("Id");
                        tp.HasKey("Id");
                    });
        }
    }
}