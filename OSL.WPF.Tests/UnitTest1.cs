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
using OSL.Common.Service;
using OSL.Common.Service.Importer;
using OSL.Common.Tests.Service;
using OSL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OSL.WPF.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly ILoggerService _LoggerService;

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
            FitLogImporter importer = new FitLogImporter(_LoggerService);

            List<ActivityEntity> activities = new List<ActivityEntity>();
            var athlete = new AthleteEntity(activities, "Sample", "1");
            dbContext.Athletes.Add(athlete);

            using (FileStream fs = File.OpenRead(path))
            {
                foreach (var activity in importer.ImportActivitiesStream(fs, new Dictionary<string, ACTIVITY_SPORT> {
                { "e41b80e4-fa5f-48e3-95be-d0e66b72ab7c", ACTIVITY_SPORT.BIKING},
                { "eca38408-cb82-42ed-b242-166b43b785a6",ACTIVITY_SPORT.RUNNING},
                { "6f2fdaf9-4c5a-4c2c-a4fa-5be42e9733dd",ACTIVITY_SPORT.SWIMMING} }))
                {
                    athlete.Activities.Add(activity);
                }
            }
            dbContext.SaveChanges();
            var count = dbContext.Athletes.CountAsync().GetAwaiter().GetResult();
            Assert.AreEqual(1, count);

            count = dbContext.Athletes.Include(a => a.Activities).CountAsync().GetAwaiter().GetResult();
            Assert.AreEqual(1, count);

            var activitiesRead = dbContext.Athletes.Include(a => a.Activities).ToList()[0].Activities;
            Assert.IsTrue(activitiesRead.Count > 0);

            var tracksPointsRead = dbContext.Athletes
                .FirstOrDefault()
                .Activities
                .FirstOrDefault()
                .Track
                .TrackPoints;
            Assert.IsTrue(tracksPointsRead.Count > 0);

            //dbContext.Database.EnsureDeleted();
        }
    }
}
