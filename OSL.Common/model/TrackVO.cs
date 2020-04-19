using GeoSports.Common.model.scaffholding;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace GeoSports.Common.model
{
    public class TrackVO : ValueObjectBase
    {
        public ImmutableList<TrackPointVO> TrackPoints { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return TrackPoints;
        }

        public sealed class Builder : ValueObjectBuilderBase<TrackVO>
        {
            private TrackVO _instance = new TrackVO();

            protected override TrackVO GetInstance()
            {
                _instance.TrackPoints = TrackPoints.ToImmutable();
                return _instance;
            }

            public ImmutableList<TrackPointVO>.Builder TrackPoints { get; private set; } = ImmutableList.CreateBuilder<TrackPointVO>();
        }
    }
}
