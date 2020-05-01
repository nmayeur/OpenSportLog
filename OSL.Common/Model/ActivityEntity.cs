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

namespace OSL.Common.Model
{

    public class ActivityEntity : ModelBase
    {
        private string _Id;
        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                NotifyPropertyChanged("Id");
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

        private TrackEntity _Track;
        public TrackEntity Track
        {
            get { return _Track; }
            set
            {
                _Track = value;
                NotifyPropertyChanged("Track");
            }
        }

        public sealed class Builder : BuilderBase<ActivityEntity>
        {
            private ActivityEntity _instance = new ActivityEntity();

            protected override ActivityEntity GetInstance()
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

            public TrackEntity Track
            {
                get { return _instance.Track; }
                set { _instance.Track = value; }
            }
        }
    }
}
