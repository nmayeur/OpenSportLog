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
using GeoSports.Common.Model.Scaffholding;
using OSL.Common.Model.Scaffholding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GeoSports.Common.Model
{
    public class TrackEntity : ModelBase
    {
        private ObservableCollection<TrackPointVO> _TrackPoints;
        public ObservableCollection<TrackPointVO> TrackPoints
        {
            get { return _TrackPoints; }
            set { _TrackPoints = value; }
        }

        public sealed class Builder : BuilderBase<TrackEntity>
        {
            private TrackEntity _instance = new TrackEntity();
            private List<TrackPointVO> _TrackPoints = new List<TrackPointVO>();

            protected override TrackEntity GetInstance()
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
