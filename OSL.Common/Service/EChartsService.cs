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
        }

        public string SerializeAthleteData(IEnumerable<ActivityEntity> activities)
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTimeOffset StartingDate = now.AddYears(-2);
            DateTimeOffset EndingDate = now;

            //calculate the number of months
            var MonthsCount = _MonthDifference(EndingDate, StartingDate);
            object[] stat = new object[MonthsCount];

            var startSlice = StartingDate.Year * 100 + StartingDate.Month;
            for (var i = startSlice; i <= EndingDate.Year * 100 + EndingDate.Month; i++)
            {
                activities.Where(a => a.Time.Year * 100 + a.Time.Month == i).Sum(a=>a.Calories);
                stat[i-startSlice] = new _AthleteStatData
                {
                    time = StartingDate.AddMonths(i).ToUnixTimeMilliseconds(),
                    //hr= activities.Where(a => a.Time.Year * 100 + a.Time.Month == i).Sum(a => a.Tracks)
                };
            }
            var json = JsonConvert.SerializeObject(stat);
            return json;
        }

        private int _MonthDifference(DateTimeOffset lValue, DateTimeOffset rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
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
