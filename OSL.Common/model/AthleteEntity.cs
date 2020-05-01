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
    public class AthleteEntity : ModelBase
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

        private ObservableCollection<ActivityEntity> _Activities;
        public ObservableCollection<ActivityEntity> Activities
        {
            get { return _Activities; }
            set { _Activities = value; }
        }

        /// <summary>
        /// Entity Framework (proxy) constructor
        /// </summary>
        public AthleteEntity() { }

        public AthleteEntity(IList<ActivityEntity> activities, string name, string id)
        {
            _Activities = new ObservableCollection<ActivityEntity>(activities);
            Name = name;
            Id = id;
        }
    }
}
