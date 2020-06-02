using CefSharp.Wpf;
using GalaSoft.MvvmLight;
using OSL.WPF.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSL.WPF.ViewModel
{
    public class AthleteStatsVM : ViewModelBase
    {

        private IWpfWebBrowser _WebBrowserStats;
        public IWpfWebBrowser WebBrowserStats
        {
            get { return _WebBrowserStats; }
            set
            {
                Set(ref _WebBrowserStats, value);
                if (_WebBrowserStats != null) _WebBrowserStats.DownloadHandler = new OSLCefDownloadHandler();
            }
        }

    }
}
