using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace OSL.Common.Service
{
    public interface IGeoJsonConverter
    {
        string GetGeoJsonFromTrackPoints(IEnumerable<TrackPointVO> TrackPoints);
    }
}
