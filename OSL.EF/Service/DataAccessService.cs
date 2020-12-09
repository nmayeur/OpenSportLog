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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using OSL.Common.Model;
using OSL.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace OSL.EF.Service
{
    public class DataAccessService : IDataAccessService
    {
        private OpenSportLogContext _DbContext;
        public OpenSportLogContext DbContext
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

            _DbContext = new OpenSportLogContext(configuration);
            if (ForceNew)
            {
                DbContext.Database.EnsureDeleted();
            }
            DbContext.Database.Migrate();
        }

        public event EventHandler<IsDirtyEventArgs> IsDirtyEvent;

        private bool _IsDirty = false;
        public bool IsDirty
        {
            get => _IsDirty;
            set
            {
                _IsDirty = value;
                IsDirtyEvent?.Invoke(this, new IsDirtyEventArgs(IsDirty: value));
            }
        }

        public IList<AthleteEntity> GetAthletes()
        {
            return DbContext.Athletes.ToList();
        }

        public void SaveData()
        {
            DbContext.SaveChanges();
            IsDirty = false;
        }

        public void AddAthlete(AthleteEntity athlete)
        {
            DbContext.Athletes.Add(athlete);
            IsDirty = true;
        }

        public IList<ActivityEntity> GetActivitiesForAthlete(AthleteEntity athlete)
        {
            if (athlete == null) return new List<ActivityEntity>();

            if (DbContext.Entry(athlete).State == EntityState.Unchanged || DbContext.Entry(athlete).State == EntityState.Modified)
            {
                DbContext.Entry(athlete).Collection(a => a.Activities).Load();
            }
            athlete.Activities.CollectionChanged += (s, e) => IsDirty = true;
            return athlete.Activities.OrderByDescending(x => x.Time).ToList();
        }

        public IEnumerable<TrackEntity> GetActivityTracks(ActivityEntity activity)
        {
            if (activity == null) return new List<TrackEntity>();

            if (DbContext.Entry(activity).State == EntityState.Unchanged || DbContext.Entry(activity).State == EntityState.Modified)
            {
                DbContext.Entry(activity).Collection(a => a.Tracks).Query().Include(t => t.TrackSegments).Load();
            }

            if (activity.Tracks == null) return new List<TrackEntity>();
            return activity.Tracks.ToList();
        }

        public IEnumerable<TrackEntity> GetActivitiesTracks(IEnumerable<ActivityEntity> activities)
        {
            if (activities == null) return new List<TrackEntity>();
            return activities.ToList().SelectMany(a => GetActivityTracks(a));
        }

        public void DeleteActivities(IList<ActivityEntity> activities)
        {
            DbContext.RemoveRange(activities);
            IsDirty = true;
        }
    }
}
