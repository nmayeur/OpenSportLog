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

namespace OSL.WPF.ViewModel
{
    public class ActivityDetailsVM : ViewModelBase
    {
        private readonly IDataAccessService _DbAccess;
        public ActivityDetailsVM(IDataAccessService DbAccess)
        {
            _DbAccess = DbAccess;
            Messenger.Default.Register<NotificationMessage<ActivityEntity>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.SELECTED)
                {
                    SelectedActivity = message.Content;
                }
            });
        }

        #region Data
        private ActivityEntity _SelectedActivity;
        public ActivityEntity SelectedActivity
        {
            get => _SelectedActivity;
            set
            {
                Set(() => SelectedActivity, ref _SelectedActivity, value);
            }
        }
        #endregion
    }
}
