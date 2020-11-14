using CefSharp.Wpf;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OSL.Common.Model;
using OSL.Common.Service;
using OSL.WPF.Utils;
using OSL.WPF.ViewModel.Scaffholding;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OSL.WPF.ViewModel
{
    public class AthleteStatsVM : BrowserViewModel
    {

        private readonly IEChartsService _EChartsService;
        private IWpfWebBrowser _WebBrowserStats;

        public AthleteStatsVM(IEChartsService EChartsService)
        {
            _Logger = NLog.LogManager.GetCurrentClassLogger();
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
                var serializedActivities = _EChartsService.SerializeAthleteData(_Activities);
                ExecuteJavaScript(WebBrowserStats, $"OSL.drawChart({serializedActivities})");
            }
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
            var serializedActivities = _EChartsService.SerializeAthleteData(_Activities);
            ExecuteJavaScript(WebBrowserStats, $"OSL.drawChart({serializedActivities})");
        }
        #endregion

    }
}
