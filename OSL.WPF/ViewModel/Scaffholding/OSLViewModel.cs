using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSL.WPF.ViewModel.Scaffholding
{
    public abstract class OSLViewModel : ViewModelBase
    {
        protected NLog.ILogger _Logger;

        public OSLViewModel()
        {
        }
    }
}
