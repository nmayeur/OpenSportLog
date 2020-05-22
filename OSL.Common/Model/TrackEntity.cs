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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OSL.Common.Model
{
    public class TrackEntity : ModelBase
    {
        private ObservableCollection<TrackSegmentEntity> _TrackSegments;
        public virtual ObservableCollection<TrackSegmentEntity> TrackSegments
        {
            get { return _TrackSegments; }
            set { _TrackSegments = value; }
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

        public sealed class Builder : BuilderBase<TrackEntity>
        {
            private TrackEntity _instance = new TrackEntity();
            private List<TrackSegmentEntity> _TrackSegments = new List<TrackSegmentEntity>();

            protected override TrackEntity GetInstance()
            {
                _instance.TrackSegments = new ObservableCollection<TrackSegmentEntity>(_TrackSegments);
                return _instance;
            }

            public List<TrackSegmentEntity> TrackSegments
            {
                get { return _TrackSegments; }
                set { _TrackSegments = value; }
            }

            public string Name
            {
                get { return _instance.Name; }
                set { _instance.Name = value; }
            }
        }
    }
}
