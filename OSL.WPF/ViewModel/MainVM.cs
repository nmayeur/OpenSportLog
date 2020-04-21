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
using GeoSports.WPF.ViewModel.Scaffholding;
using System;

namespace GeoSports.WPF.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainVM : ViewModelBase
    {
        public class CloseNotificationEventArgs : EventArgs
        {
            public CloseNotificationEventArgs()
            {

            }
            public string Message { get; protected set; }
        }

        public event EventHandler<CloseNotificationEventArgs> CloseApp;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainVM()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            Messenger.Default.Register<CloseDialogMessage>(this, nm =>
            {
                CloseApp?.Invoke(this, new CloseNotificationEventArgs());
            });
        }
    }
}