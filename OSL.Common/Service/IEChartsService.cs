using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OSL.Common.Service
{
    public interface IEChartsService
    {
        string SerializeTrackDatas(IEnumerable<TrackPointVO> trackPoints);
        string SerializeAthleteData(IEnumerable<ActivityEntity> activities);
    }
}
