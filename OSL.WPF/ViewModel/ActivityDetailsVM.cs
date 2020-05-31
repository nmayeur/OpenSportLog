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
using System.ComponentModel;
using OSL.Common.Model.ECharts;

namespace OSL.WPF.ViewModel
{
    public class ActivityDetailsVM : ViewModelBase
    {
        private static readonly NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IDataAccessService _DbAccess;
        private readonly IEChartsService _EChartsService;
        private readonly IGeoJsonConverter _GeoJsonConverter;
        public ActivityDetailsVM(IDataAccessService DbAccess, IEChartsService EChartsService, IGeoJsonConverter GeoJsonConverter)
        {
            _DbAccess = DbAccess;
            _EChartsService = EChartsService;
            _GeoJsonConverter = GeoJsonConverter;
            Messenger.Default.Register<NotificationMessage<ActivityEntity>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.SELECTED)
                {
                    SelectedActivity = message.Content;
                }
            });
        }

        #region Zooming

        public void OnDataZoom(object sender, CancelEventArgs e)
        {
            var dataZoom = sender as DataZoomModel;
            var trackPoints = _SelectedActivity?.Tracks.ElementAtOrDefault(0)?.TrackSegments.ElementAtOrDefault(0)?.TrackPoints?.OrderBy(tp => tp.Time);
            if (trackPoints != null)
            {
                var geoJson = _GeoJsonConverter.GetGeoJsonZoomFromTrackPoints(trackPoints, dataZoom.startTime, dataZoom.endTime);
                _ExecuteJavaScript(_WebBrowser, $"OSL.drawZoomedRoute({geoJson})");
            }
        }
        #endregion

        #region Data
        private ActivityEntity _SelectedActivity;
        public ActivityEntity SelectedActivity
        {
            get => _SelectedActivity;
            set
            {
                Set(() => SelectedActivity, ref _SelectedActivity, value);
                _DbAccess.GetActivityTracks(SelectedActivity);
                var trackPoints = _SelectedActivity?.Tracks.ElementAtOrDefault(0)?.TrackSegments.ElementAtOrDefault(0)?.TrackPoints?.OrderBy(tp => tp.Time);
                var trackPoint = trackPoints?.ElementAtOrDefault(0);
                var latitude = trackPoint?.Latitude ?? 48.8534;
                var longitude = trackPoint?.Longitude ?? 2.3488;
                FormattableString command = $"OSL.goToCoordinates({latitude:N6},{longitude:N6})";
                var enCulture = CultureInfo.GetCultureInfo("en-US");
                _ExecuteJavaScript(_WebBrowser, command.ToString(enCulture));

                command = $"OSL.setMarker({latitude:N6},{longitude:N6},OSL.START_MARKER)";
                _ExecuteJavaScript(_WebBrowser, command.ToString(enCulture));

                if (trackPoints != null)
                {
                    var geoJson = _GeoJsonConverter.GetGeoJsonFromTrackPoints(trackPoints);
                    command = $"OSL.drawRoute({geoJson})";
                    _ExecuteJavaScript(_WebBrowser, command.ToString(enCulture));

                    var serializedPoints = _EChartsService.SerializeTrackDatas(trackPoints);
                    _ExecuteJavaScript(_WebBrowserActivityCharts, $"OSL.loadData({serializedPoints})");
                }
                else
                {
                    _ExecuteJavaScript(_WebBrowser, "OSL.cleanMap()");
                }
            }
        }

        private IWpfWebBrowser _WebBrowser;
        public IWpfWebBrowser WebBrowser
        {
            get { return _WebBrowser; }
            set { Set(ref _WebBrowser, value); }
        }

        private IWpfWebBrowser _WebBrowserActivityCharts;
        public IWpfWebBrowser WebBrowserActivityCharts
        {
            get { return _WebBrowserActivityCharts; }
            set { Set(ref _WebBrowserActivityCharts, value); }
        }
        #endregion

        #region Utils
        private void _ExecuteJavaScript(IWpfWebBrowser browser, string s)
        {
            try
            {
                _Logger.Debug($"Execute Javascript {s}");
                browser.ExecuteScriptAsync(s);
            }
            catch (Exception e)
            {
                MessageBox.Show("Error while executing Javascript: " + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
    }
}
