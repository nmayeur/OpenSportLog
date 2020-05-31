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
            //return "[{ x: 0, y: 20 }, { x: 150, y: 150 }, { x: 300, y: 100 }, { x: 450, y: 20 }, { x: 600, y: 130 }]";
            object[] points = new object[trackPoints.Count() + 1];
            points[0] = new object[] { "time", "hr", "cadence", "elevation" };
            trackPoints.Select((tp, index) =>
             {
                 object[] array = { tp.Time.ToUnixTimeSeconds(), tp.HeartRate, tp.Cadence, tp.Elevation };
                 return array;
             }).ToArray().CopyTo(points, 1);
            var json = JsonConvert.SerializeObject(points);
            return json;
        }
    }
}
