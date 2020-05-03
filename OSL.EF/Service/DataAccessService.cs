﻿/* Copyright 2020 Nicolas Mayeur

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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OSL.Common.Model;
using OSL.Common.Service;
using System.Collections.Generic;
using System.Linq;

namespace OSL.EF.Service
{
    public class DataAccessService : IDataAccessService
    {
        private GeoSportsContext _DbContext;
        public GeoSportsContext DbContext
        {
            get => _DbContext;
        }

        public void OpenDatabase(string DatabasePath, bool ForceNew = false)
        {
            var connectionString = string.Format("Data Source={0}", DatabasePath);
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                        { "ConnectionStrings:Default", connectionString }
                }).Build();

            _DbContext = new GeoSportsContext(configuration);
            if (ForceNew)
            {
                DbContext.Database.EnsureDeleted();
            }
            DbContext.Database.Migrate();
        }

        public IList<AthleteEntity> GetAthletes()
        {
            return DbContext.Athletes.ToList();
        }

        public void SaveData()
        {
            DbContext.SaveChanges();
        }

        public void AddAthlete(AthleteEntity athlete)
        {
            _DbContext.Athletes.Add(athlete);
        }
    }
}