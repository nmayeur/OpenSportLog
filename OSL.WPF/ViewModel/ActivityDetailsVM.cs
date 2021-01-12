/* Copyright 2021 Nicolas Mayeur

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
using CefSharp.Wpf;
using GalaSoft.MvvmLight.Messaging;
using OSL.Common.Model;
using OSL.Common.Model.ECharts;
using OSL.Common.Service;
using OSL.WPF.Utils;
using OSL.WPF.ViewModel.Scaffholding;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OSL.WPF.ViewModel
{
    public class ActivityDetailsVM : BrowserViewModel
    {
        private readonly IDataAccessService _DbAccess;
        private readonly IEChartsService _EChartsService;
        private readonly IGeoJsonConverter _GeoJsonConverter;
        public ActivityDetailsVM(IDataAccessService DbAccess, IEChartsService EChartsService, IGeoJsonConverter GeoJsonConverter)
        {
            _Logger = NLog.LogManager.GetCurrentClassLogger();
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

        CancellationTokenSource zoomCancelToken = null;
        /// <summary>
        /// Delay, in milliseconds, for waiting that all zoom events are received. Only proceed with the last event received.
        /// </summary>
        private static readonly int WAIT_FOR_ZOOM = 2000;

        public void OnDataZoom(object sender, CancelEventArgs e)
        {
            if (zoomCancelToken != null)
            {
                zoomCancelToken.Cancel();
            }
            zoomCancelToken = new CancellationTokenSource();
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromMilliseconds(WAIT_FOR_ZOOM), zoomCancelToken.Token);
                _Logger.Debug("OnDataZoom called");
                var dataZoom = sender as DataZoomModel;
                var trackPoints = _SelectedActivity?.Tracks.ElementAtOrDefault(0)?.TrackSegments.ElementAtOrDefault(0)?.TrackPoints?.OrderBy(tp => tp.Time);
                if (trackPoints != null)
                {
                    var geoJson = _GeoJsonConverter.GetGeoJsonZoomFromTrackPoints(trackPoints, dataZoom.startTime, dataZoom.endTime);
                    ExecuteJavaScript(_WebBrowser, $"OSL.drawZoomedRoute({geoJson})");
                }
            });
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
                ExecuteJavaScript(_WebBrowser, command.ToString(enCulture));

                command = $"OSL.setMarker({latitude:N6},{longitude:N6},OSL.START_MARKER)";
                ExecuteJavaScript(_WebBrowser, command.ToString(enCulture));

                if (trackPoints != null)
                {
                    var geoJson = _GeoJsonConverter.GetGeoJsonFromTrackPoints(trackPoints);
                    command = $"OSL.drawRoute({geoJson})";
                    ExecuteJavaScript(_WebBrowser, command.ToString(enCulture));

                    ExecuteJavaScript(_WebBrowserActivityCharts, $"OSL.clear()");
                    var serializedPoints = _EChartsService.SerializeTrackDatas(trackPoints);
                    ExecuteJavaScript(_WebBrowserActivityCharts, $"OSL.loadData({serializedPoints})");
                }
                else
                {
                    ExecuteJavaScript(_WebBrowser, "OSL.cleanMap()");
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
            set
            {
                Set(ref _WebBrowserActivityCharts, value);
                if (_WebBrowserActivityCharts != null) _WebBrowserActivityCharts.DownloadHandler = new OSLCefDownloadHandler();
            }
        }
        #endregion

    }
}
