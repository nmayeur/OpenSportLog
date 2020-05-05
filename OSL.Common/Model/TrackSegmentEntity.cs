using OSL.Common.Model.Scaffholding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OSL.Common.Model
{
    public class TrackSegmentEntity : ModelBase
    {
        private ObservableCollection<TrackPointVO> _TrackPoints;
        public ObservableCollection<TrackPointVO> TrackPoints
        {
            get { return _TrackPoints; }
            set { _TrackPoints = value; }
        }
        public sealed class Builder : BuilderBase<TrackSegmentEntity>
        {
            private TrackSegmentEntity _instance = new TrackSegmentEntity();
            private List<TrackPointVO> _TrackPoints = new List<TrackPointVO>();

            protected override TrackSegmentEntity GetInstance()
            {
                _instance.TrackPoints = new ObservableCollection<TrackPointVO>(_TrackPoints);
                return _instance;
            }

            public List<TrackPointVO> TrackPoints
            {
                get { return _TrackPoints; }
                set { _TrackPoints = value; }
            }
        }
    }
}
