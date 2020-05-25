using OSL.Common.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OSL.Common.Service
{
    public interface ID3jsService
    {
        string SerializeTrackDatas(IEnumerable<TrackPointVO> trackPoints);
    }
}
