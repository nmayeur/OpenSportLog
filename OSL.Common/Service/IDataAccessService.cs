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
using System.Collections.Generic;

namespace OSL.Common.Service
{
    public interface IDataAccessService
    {
        void OpenDatabase(string DatabasePath, bool ForceNew = false);
        IList<AthleteEntity> GetAthletes();
        IList<ActivityEntity> GetActivitiesForAthlete(int athleteId);
        IList<TrackEntity> GetActivityTracks(ActivityEntity activity);
        void SaveData();
        void AddAthlete(AthleteEntity athlete);
    }
}
