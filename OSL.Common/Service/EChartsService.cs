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

        public string SerializeAthleteData(IEnumerable<ActivityEntity> activities)
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTimeOffset StartingDate = new DateTime(now.Year - 2, 1, 1);
            DateTimeOffset EndingDate = now;

            //calculate the number of months
            var MonthsCount = _MonthDifference(EndingDate, StartingDate);
            object[] data = new object[MonthsCount + 1];
            data[0] = new string[] { "time", "hr", "calories", "power", "temperature" };

            for (var i = 0; i < (EndingDate.Year - StartingDate.Year) * 12 + EndingDate.Month; i++)
            {
                var monthActivities = activities.Where(a =>
                    (a.Time.Year - StartingDate.Year) * 12 + a.Time.Month - 1 == i
                ).ToList();
                var monthTrackPoints = monthActivities
                    .Where(a => a.Tracks != null).SelectMany(a => a.Tracks)
                    .Where(t => t.TrackSegments != null).SelectMany(t => t.TrackSegments)
                    .Where(s => s.TrackPoints != null).SelectMany(s => s.TrackPoints)
                    .DefaultIfEmpty(new TrackPointVO.Builder { Cadence = 0, Elevation = 0, HeartRate = 0, Latitude = 0, Longitude = 0, Power = 0, Temperature = 0 });
                //TODO : tracks not loaded
                data[i + 1] = monthActivities.Count == 0 ? new object[] { StartingDate.AddMonths(i).ToUnixTimeMilliseconds(), 0, 0, 0 } : new object[]
                {
                    StartingDate.AddMonths(i).ToUnixTimeMilliseconds(),
                    (int)Math.Round(monthTrackPoints.Average(p=>p.HeartRate)),
                    monthActivities.Sum(a => a.Calories),
                    (int)Math.Round(monthActivities.Average(a => a.Power)),
                    (int)Math.Round(monthActivities.Average(a => a.Temperature))
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
