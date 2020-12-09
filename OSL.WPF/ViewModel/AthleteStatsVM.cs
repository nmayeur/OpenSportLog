using CefSharp.Wpf;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.WPF.Utils;
using OSL.WPF.ViewModel.Scaffholding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OSL.WPF.ViewModel
{
    public class AthleteStatsVM : BrowserViewModel
    {

        private readonly IEChartsService _EChartsService;
        private IWpfWebBrowser _WebBrowserStats;
        private readonly IDataAccessService _DbAccess;

        public AthleteStatsVM(IDataAccessService DbAccess, IEChartsService EChartsService)
        {
            _Logger = NLog.LogManager.GetCurrentClassLogger();
            _DbAccess = DbAccess;
            _EChartsService = EChartsService;
            Messenger.Default.Register<NotificationMessage<IList<ActivityEntity>>>(this, message =>
            {
                if (message.Notification == MessengerNotifications.LOADED)
                {
                    if (message.Content == null)
                    {
                        Activities = new ObservableCollection<ActivityEntity>();
                    }
                    else
                    {
                        Activities = new ObservableCollection<ActivityEntity>(message.Content);
                    }
                }
            });
        }
        #region Data

        ObservableCollection<ActivityEntity> _Activities = new ObservableCollection<ActivityEntity>() { };

        public ObservableCollection<ActivityEntity> Activities
        {
            get => _Activities;
            private set
            {
                Set(() => Activities, ref _Activities, value);

                var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                DateTimeOffset StartingDate = new DateTime(now.Year - 2, 1, 1);
                DateTimeOffset EndingDate = now;

                //_LoadTracksForActivities(StartingDate, EndingDate);
                var serializedActivities = _EChartsService.SerializeAthleteData(_Activities, new SerializeAthleteDataConfig { StartingDate = StartingDate, EndingDate = EndingDate });
                ExecuteJavaScript(WebBrowserStats, $"OSL.drawChart({serializedActivities})");
            }
        }

        private void _LoadTracksForActivities(DateTimeOffset StartingDate, DateTimeOffset EndingDate)
        {
            var activities = _Activities.Where(a =>
                          a.Time >= StartingDate && a.Time <= EndingDate);
            _DbAccess.GetActivitiesTracks(activities);
        }

        public IWpfWebBrowser WebBrowserStats
        {
            get { return _WebBrowserStats; }
            set
            {
                Set(ref _WebBrowserStats, value);
                if (_WebBrowserStats != null)
                {
                    _WebBrowserStats.DownloadHandler = new OSLCefDownloadHandler();
                }
            }
        }

        private bool _IsStartEnabled = true;
        public bool IsStartEnabled
        {
            get => _IsStartEnabled;
            set
            {
                Set(() => IsStartEnabled, ref _IsStartEnabled, value);
            }
        }


        #endregion

        #region StartCommand
        private RelayCommand _StartCommand;
        public RelayCommand StartCommand
        {
            get
            {
                return _StartCommand ??
                    (_StartCommand = new RelayCommand(
                        () => { _Start(); },
                        () => { return IsStartEnabled; }
                        ));
            }
        }

        private void _Start()
        {
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTimeOffset StartingDate = new DateTime(now.Year - 2, 1, 1);
            DateTimeOffset EndingDate = now;
            //_LoadTracksForActivities(StartingDate, EndingDate);
            var serializedActivities = _EChartsService.SerializeAthleteData(_Activities, new SerializeAthleteDataConfig { StartingDate = StartingDate, EndingDate = EndingDate });
            ExecuteJavaScript(WebBrowserStats, $"OSL.drawChart({serializedActivities})");
        }
        #endregion

    }
}
