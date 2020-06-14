using Newtonsoft.Json;
using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OSL.Common.Service
{
    public class EChartsService : IEChartsService
    {
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
