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
using CefSharp;
using CefSharp.Wpf;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.WPF.ViewModel.Scaffholding;
using System;
using System.Linq;
using System.Globalization;
using System.Windows;

namespace OSL.WPF.ViewModel
{
    public class ActivityDetailsVM : ViewModelBase
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

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
                var trackPoints = _SelectedActivity?.Tracks.ElementAtOrDefault(0)?.TrackSegments.ElementAtOrDefault(0)?.TrackPoints?.OrderBy(tp => tp.Time);
                var trackPoint = trackPoints?.ElementAtOrDefault(0);
                var latitude = trackPoint?.Latitude ?? 48.8534;
                var longitude = trackPoint?.Longitude ?? 2.3488;
                FormattableString command = $"OSL.goToCoordinates({latitude:N6},{longitude:N6})";
                var enCulture = CultureInfo.GetCultureInfo("en-US");
                _ExecuteJavaScript(command.ToString(enCulture));

                command = $"OSL.setMarker({latitude:N6},{longitude:N6},OSL.START_MARKER)";
                _ExecuteJavaScript(command.ToString(enCulture));

                if (trackPoints != null)
                {
                    var latlngs = "[";
                    foreach (var tp in trackPoints)
                    {
                        command = $"[{tp.Latitude:N6},{tp.Longitude:N6}],";
                        latlngs += command.ToString(enCulture);
                    }
                    command = $"OSL.drawRoute({latlngs}])";
                    _ExecuteJavaScript(command.ToString(enCulture));
                }
                else
                {
                    _ExecuteJavaScript("OSL.cleanMap()");
                }
            }
        }

        private IWpfWebBrowser _WebBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { Set(ref _WebBrowser, value); }
        }
        #endregion

        #region ExecuteJavaScriptCommand
        public RelayCommand<string> _ExecuteJavaScriptCommand { get; private set; }
        public RelayCommand<string> ExecuteJavaScriptCommand
        {
            get
            {
                return _ExecuteJavaScriptCommand ??
                    (_ExecuteJavaScriptCommand = new RelayCommand<string>(
                        _ExecuteJavaScript, s => !String.IsNullOrWhiteSpace(s)
                        ));
            }
        }
        private void _ExecuteJavaScript(string s)
        {
            try
            {
                _Logger.Debug($"Execute Javascript {s}");
                _WebBrowser.ExecuteScriptAsync(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while executing Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
