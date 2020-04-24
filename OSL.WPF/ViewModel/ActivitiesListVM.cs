﻿/* Copyright 2020 Nicolas Mayeur

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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using GeoSports.Common.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace GeoSports.WPF.ViewModel
{
    public class ActivitiesListVM : ViewModelBase
    {

        public ActivitiesListVM()
        {
            Messenger.Default.Register<NotificationMessage<AthleteEntity>>(this, message =>
            {
                SelectedAthlete = message.Content;
            });

        }

        #region Data
        private AthleteEntity _SelectedAthlete;
        public AthleteEntity SelectedAthlete
        {
            get => _SelectedAthlete;
            set
            {
                Set(() => SelectedAthlete, ref _SelectedAthlete, value);
                SelectedAthlete.Activities.ToList<ActivityVO>().ForEach(a => Activities.Add(a));
            }
        }

        ObservableCollection<ActivityVO> _Activities = new ObservableCollection<ActivityVO>() { };

        public ObservableCollection<ActivityVO> Activities
        {
            get => _Activities;
            private set { Set(() => Activities, ref _Activities, value); }
        }
        #endregion
    }
}
