/* Copyright 2021 Nicolas Mayeur

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
using Newtonsoft.Json;
using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OSL.Common.Service
{
    public class EChartsService : IEChartsService
    {

        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

        public string SerializeAthleteData(IEnumerable<ActivityEntity> activities, SerializeAthleteDataConfig config)
        {
            //calculate the number of months
            var MonthsCount = _MonthDifference(config.EndingDate, config.StartingDate);
            object[] data = new object[MonthsCount + 1];
            data[0] = new string[] { "time", "hr", "calories", "power", "temperature" };

            for (var i = 0; i < (config.EndingDate.Year - config.StartingDate.Year) * 12 + config.EndingDate.Month; i++)
            {
                var monthActivities = activities.Where(a =>
                    (a.Time.Year - config.StartingDate.Year) * 12 + a.Time.Month - 1 == i
                ).ToList();

                int totalWeight = monthActivities.Sum(a => a.TracksPointsCount);
                if (totalWeight == 0) totalWeight = 1; //Should not occur, but avoids DivedByZero in case of corrupted database
                data[i + 1] = monthActivities.Count == 0 ? new object[] { config.StartingDate.AddMonths(i).ToUnixTimeMilliseconds(), 0, 0, 0 } : new object[]
               {
                    config.StartingDate.AddMonths(i).ToUnixTimeMilliseconds(),
                    (int)Math.Round((decimal)monthActivities.Sum(a=>a.HeartRate*a.TracksPointsCount)/totalWeight),
                    monthActivities.Sum(a => a.Calories),
                    (int)Math.Round((decimal)monthActivities.Sum(a=>a.Power*a.TracksPointsCount)/totalWeight),
                    (int)Math.Round((decimal)monthActivities.Sum(a=>a.Temperature*a.TracksPointsCount)/totalWeight)
               };
            }
            var ret = new
            {
                data = data,
                labels = new { hr = "HR", calories = "Calories", power = "Power", temperature = "Temperature" },
                axis = new { hr = "left", calories = "right", power = "left", temperature = "left" }
            };
            var json = JsonConvert.SerializeObject(ret);
            _Logger.Debug($"Serialized data for athlete stats : {json}");
            return json;
        }

        private int _MonthDifference(DateTimeOffset lValue, DateTimeOffset rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year) + 1;
        }

        public string SerializeTrackDatas(IEnumerable<TrackPointVO> trackPoints)
        {
            object[] points = new object[trackPoints.Count() + 1];
            points[0] = new object[] { "time", "hr", "cadence", "elevation", "power", "temperature" };
            trackPoints.Select((tp, index) =>
             {
                 object[] array = { tp.Time.ToUnixTimeMilliseconds(), tp.HeartRate, tp.Cadence, tp.Elevation, tp.Power, tp.Temperature };
                 return array;
             }).ToArray().CopyTo(points, 1);
            var json = JsonConvert.SerializeObject(points);
            return json;
        }
    }
}
