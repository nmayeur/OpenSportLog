using Newtonsoft.Json;
using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OSL.Common.Service
{
    public class D3jsService : ID3jsService
    {
        public string SerializeTrackDatas(IEnumerable<TrackPointVO> trackPoints)
        {
            //return "[{ x: 0, y: 20 }, { x: 150, y: 150 }, { x: 300, y: 100 }, { x: 450, y: 20 }, { x: 600, y: 130 }]";
            var points=trackPoints.Select((tp, index) => new { x = index, y = tp.HeartRate }).ToArray();
            var json = JsonConvert.SerializeObject(points);
            return json;
        }
    }
}
