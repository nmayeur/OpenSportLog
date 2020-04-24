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
using GeoSports.Common.Model;
using GeoSports.Common.Service;
using GeoSports.Common.Service.Importer;
using GeoSports.Common.Tests.Service;
using GeoSports.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeoSports.WPF.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private ILoggerService _LoggerService;

        public UnitTest1()
        {
            _LoggerService = new TestLoggerService(TestLoggerService.LEVEL.DEBUG);
        }

        [TestMethod]
        public void TestCreateDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:Default", "Data Source=geosports1.db" }
                }).Build();

            var dbContext = new GeoSportsContext(configuration);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
            dbContext.Database.EnsureDeleted();
        }

        [TestMethod]
        public void TestLoadFileToDatabase()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()
                {
                    { "ConnectionStrings:Default", "Data Source=geosports.db" }
                }).Build();

            var dbContext = new GeoSportsContext(configuration);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();

            string path = @"data\data_tiny.fitlog";
            FitLogImporter importer = new FitLogImporter(_LoggerService, new Dictionary<string, ActivityVO.ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ActivityVO.ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ActivityVO.ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ActivityVO.ACTIVITY_SPORT.SWIMMING} });

            List<ActivityVO> activities = new List<ActivityVO>();
            var athlete = new AthleteEntity(activities, "Sample", "1");
            dbContext.Athletes.Add(athlete);

            using (FileStream fs = File.OpenRead(path))
            {
                foreach (var activity in importer.ImportActivitiesStream(fs))
                {
                    activities.Add(activity);
                }
            }
            dbContext.SaveChanges();
            var athletesCount = dbContext.Athletes.CountAsync().GetAwaiter().GetResult();
            Assert.AreEqual(1, athletesCount);

            //dbContext.Database.EnsureDeleted();
        }
    }
}
