using GeoSports.Common.model.scaffholding;
using System.Collections.Generic;

namespace GeoSports.Common.model
{
    public class ActivityVO : ValueObjectBase
    {
        // 0 OTHER by default
        public enum ACTIVITY_SPORT { OTHER = 0, RUNNING = 1, BIKING = 2, SWIMMING = 3, HIKING = 4 }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Location { get; private set; }
        public int Calories { get; private set; }
        public ACTIVITY_SPORT Sport { get; private set; }
        public TrackVO Track { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
            yield return Location;
            yield return Calories;
            yield return Sport;
            yield return Track;
        }

        public sealed class Builder : ValueObjectBuilderBase<ActivityVO>
        {
            private ActivityVO _instance = new ActivityVO();

            protected override ActivityVO GetInstance()
            {
                return _instance;
            }

            public string Id
            {
                get { return _instance.Id; }
                set { _instance.Id = value; }
            }
            public string Name
            {
                get { return _instance.Name; }
                set { _instance.Name = value; }
            }
            public string Location
            {
                get { return _instance.Location; }
                set { _instance.Location = value; }
            }
            public int Calories
            {
                get { return _instance.Calories; }
                set { _instance.Calories = value; }
            }
            public ACTIVITY_SPORT Sport
            {
                get { return _instance.Sport; }
                set { _instance.Sport = value; }
            }
            public TrackVO Track
            {
                get { return _instance.Track; }
                set { _instance.Track = value; }
            }
        }
    }
}
