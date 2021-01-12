/* Copyright 2021 Nicolas Mayeur

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using OSL.Common.Model.Scaffholding;
using System;
using System.Collections.Generic;

namespace OSL.Common.Model
{
    public class TrackPointVO : ValueObjectBase
    {
        public DateTimeOffset Time { get; private set; }
        public float Latitude { get; private set; }
        public float Longitude { get; private set; }
        public float Elevation { get; private set; }
        public int HeartRate { get; private set; }
        public int Cadence { get; private set; }
        public int Power { get; private set; }
        public float Temperature { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Time;
            yield return Latitude;
            yield return Longitude;
            yield return Elevation;
            yield return HeartRate;
            yield return Cadence;
            yield return Power;
            yield return Temperature;
        }

        public sealed class Builder : BuilderBase<TrackPointVO>
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
                get { return _instance.HeartRate; }
                set { _instance.HeartRate = value; }
            }
            public int Cadence
            {
                get { return _instance.Cadence; }
                set { _instance.Cadence = value; }
            }
            public int Power
            {
                get { return _instance.Power; }
                set { _instance.Power = value; }
            }
            public float Temperature
            {
                get { return _instance.Temperature; }
                set { _instance.Temperature = value; }
            }
        }
    }
}
