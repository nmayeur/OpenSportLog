using GeoSports.Common.model.scaffholding;
using System;
using System.Collections.Generic;

namespace GeoSports.Common.model
{
    public class TrackPointVO : ValueObjectBase
    {
        public DateTimeOffset Time { get; private set; }
        public float Latitude { get; private set; }
        public float Longitude { get; private set; }
        public float Elevation { get; private set; }
        public int HearRate { get; private set; }
        public int Cadence { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Time;
            yield return Latitude;
            yield return Longitude;
            yield return Elevation;
            yield return HearRate;
            yield return Cadence;
        }

        public sealed class Builder : ValueObjectBuilderBase<TrackPointVO>
        {
            private TrackPointVO _instance = new TrackPointVO();

            protected override TrackPointVO GetInstance()
            {
                return _instance;
            }

            public DateTimeOffset Time
            {
                get { return _instance.Time; }
                set { _instance.Time = value; }
            }
            public float Latitude
            {
                get { return _instance.Latitude; }
                set { _instance.Latitude = value; }
            }
            public float Longitude
            {
                get { return _instance.Longitude; }
                set { _instance.Longitude = value; }
            }
            public float Elevation
            {
                get { return _instance.Elevation; }
                set { _instance.Elevation = value; }
            }
            public int HeartRate
            {
                get { return _instance.HearRate; }
                set { _instance.HearRate = value; }
            }
            public int Cadence
            {
                get { return _instance.Cadence; }
                set { _instance.Cadence = value; }
            }
        }
    }
}
