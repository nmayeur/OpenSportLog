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

namespace OSL.EF
{
    public class OpenSportLogContext : DbContext
    {
        IConfigurationRoot _Configuration;

        public OpenSportLogContext()
        {
            _Configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:Default", "Data Source=geosports.db" }
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
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AthleteConfiguration());
        }
    }
}