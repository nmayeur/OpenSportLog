using Newtonsoft.Json;
using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OSL.Common.Service
{
    public class EChartsService : IEChartsService
    {

        private class _AthleteStatData
        {
            public double time;
            public int hr;
            public int cadence;
            public int elevation;
            public int power;
            public int temperature;
            public int distance;
            public int duration;
            public int calories;
        }

        public string SerializeAthleteData(IEnumerable<ActivityEntity> activities)
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTimeOffset StartingDate = new DateTime(now.Year - 2, 1, 1);
            DateTimeOffset EndingDate = now;

            //calculate the number of months
            var MonthsCount = _MonthDifference(EndingDate, StartingDate);
            object[] stat = new object[MonthsCount];

            for (var i = 0; i < (EndingDate.Year - StartingDate.Year) * 12 + EndingDate.Month; i++)
            {
                var monthActivities = activities.Where(a => (EndingDate.Year - a.Time.Year) * 12 + a.Time.Month == i).ToList();
                stat[i] = monthActivities.Count == 0 ? null : new _AthleteStatData
                {
                    time = StartingDate.AddMonths(i).ToUnixTimeMilliseconds(),
                    calories = monthActivities.Sum(a => a.Calories),
                    power = (int)Math.Round(monthActivities.Average(a => a.Power)),
                    temperature = (int)Math.Round(monthActivities.Average(a => a.Temperature)),
                    //hr= activities.Where(a => a.Time.Year * 100 + a.Time.Month == i).Sum(a => a.Tracks)
                };
            }
            var json = JsonConvert.SerializeObject(stat);
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
