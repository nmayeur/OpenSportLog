/* Copyright 2020 Nicolas Mayeur

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
using System.Collections.ObjectModel;

namespace OSL.Common.Model
{

    public class ActivityEntity : ModelBase
    {
        private int _Id;
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private AthleteEntity _Athlete;
        public AthleteEntity Athlete
        {
            get { return _Athlete; }
            set
            {
                _Athlete = value;
                NotifyPropertyChanged("Athlete");
            }
        }

        private string _OriginId;
        public string OriginId
        {
            get { return _OriginId; }
            set
            {
                _OriginId = value;
                NotifyPropertyChanged("OriginId");
            }
        }

        private string _OriginSystem;
        public string OriginSystem
        {
            get { return _OriginSystem; }
            set
            {
                _OriginSystem = value;
                NotifyPropertyChanged("OriginSystem");
            }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        private string _Location;
        public string Location
        {
            get { return _Location; }
            set
            {
                _Location = value;
                NotifyPropertyChanged("Location");
            }
        }

        private int _Calories;
        public int Calories
        {
            get { return _Calories; }
            set
            {
                _Calories = value;
                NotifyPropertyChanged("Calories");
            }
        }

        private int _HeartRate;
        public int HeartRate
        {
            get { return _HeartRate; }
            set
            {
                _HeartRate = value;
                NotifyPropertyChanged("HeartRate");
            }
        }

        private int _Cadence;
        public int Cadence
        {
            get { return _Cadence; }
            set
            {
                _Cadence = value;
                NotifyPropertyChanged("Cadence");
            }
        }

        private int _Power;
        public int Power
        {
            get { return _Power; }
            set
            {
                _Power = value;
                NotifyPropertyChanged("Power");
            }
        }

        private int _Temperature;
        public int Temperature
        {
            get { return _Temperature; }
            set
            {
                _Temperature = value;
                NotifyPropertyChanged("Temperature");
            }
        }

        private ACTIVITY_SPORT _Sport;
        public ACTIVITY_SPORT Sport
        {
            get { return _Sport; }
            set
            {
                _Sport = value;
                NotifyPropertyChanged("Sport");
            }
        }

        private DateTimeOffset _Time;
        public DateTimeOffset Time
        {
            get { return _Time; }
            set
            {
                _Time = value;
                NotifyPropertyChanged("Time");
            }
        }

        private TimeSpan _TimeSpan;
        public TimeSpan TimeSpan
        {
            get { return _TimeSpan; }
            set
            {
                _TimeSpan = value;
                NotifyPropertyChanged("TimeSpan");
            }
        }

        private ObservableCollection<TrackEntity> _Tracks;
        public virtual ObservableCollection<TrackEntity> Tracks
        {
            get { return _Tracks; }
            set { _Tracks = value; }
        }

        public sealed class Builder : BuilderBase<ActivityEntity>
        {
            private ActivityEntity _instance = new ActivityEntity();
            private List<TrackEntity> _Tracks = new List<TrackEntity>();

            protected override ActivityEntity GetInstance()
            {
                _instance.Tracks = new ObservableCollection<TrackEntity>(_Tracks);
                return _instance;
            }

            public int Id
            {
                get { return _instance.Id; }
                set { _instance.Id = value; }
            }

            public AthleteEntity Athlete
            {
                get { return _instance.Athlete; }
                set { _instance.Athlete = value; }
            }

            public string OriginId
            {
                get { return _instance.OriginId; }
                set { _instance.OriginId = value; }
            }

            public string OriginSystem
            {
                get { return _instance.OriginSystem; }
                set { _instance.OriginSystem = value; }
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

            public int Temperature
            {
                get { return _instance.Temperature; }
                set { _instance.Temperature = value; }
            }

            public ACTIVITY_SPORT Sport
            {
                get { return _instance.Sport; }
                set { _instance.Sport = value; }
            }

            public DateTimeOffset Time
            {
                get { return _instance.Time; }
                set { _instance.Time = value; }
            }

            public TimeSpan TimeSpan
            {
                get { return _instance.TimeSpan; }
                set { _instance.TimeSpan = value; }
            }

            public List<TrackEntity> Tracks
            {
                get { return _Tracks; }
                set { _Tracks = value; }
            }
        }
    }
}
