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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using OSL.Common.Model;
using OSL.WPF.Service;
using OSL.WPF.ViewModel.Scaffholding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OSL.WPF.ViewModel
{
    public class AthleteDetailsVM : ViewModelBase
    {

        private readonly IDataAccessService _DbAccess;
        public AthleteDetailsVM(IDataAccessService DbAccess)
        {
            _DbAccess = DbAccess;
            Messenger.Default.Register<NotificationMessage<IList<AthleteEntity>>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.LOADED)
                {
                    if (message.Content == null)
                    {
                        Athletes = new ObservableCollection<AthleteEntity>();
                    }
                    else
                    {
                        Athletes = new ObservableCollection<AthleteEntity>(message.Content);
                    }
                }
            });

        }

        #region Data
        ObservableCollection<AthleteEntity> _Athletes = new ObservableCollection<AthleteEntity>() { };

        public ObservableCollection<AthleteEntity> Athletes
        {
            get => _Athletes;
            private set { Set(() => Athletes, ref _Athletes, value); }
        }

        private AthleteEntity _SelectedAthlete;
        public AthleteEntity SelectedAthlete
        {
            get => _SelectedAthlete;
            set
            {
                Set(() => SelectedAthlete, ref _SelectedAthlete, value);
                Activities = _SelectedAthlete.Activities;
            }
        }

        private ActivityEntity _SelectedActivity;
        public ActivityEntity SelectedActivity
        {
            get => _SelectedActivity;
            set
            {
                Set(() => SelectedActivity, ref _SelectedActivity, value);
                Messenger.Default.Send(new NotificationMessage<ActivityEntity>(_SelectedActivity, MessengerNotifications.SELECTED));
            }
        }

        ObservableCollection<ActivityEntity> _Activities = new ObservableCollection<ActivityEntity>() { };

        public ObservableCollection<ActivityEntity> Activities
        {
            get => _Activities;
            private set { Set(() => Activities, ref _Activities, value); }
        }
        #endregion
    }
}
