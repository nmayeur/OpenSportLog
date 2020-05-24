using OSL.WPF.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSL.WPF.Utils
{
    public class InitSettings
    {
        public static void Init()
        {
            if (Settings.Default["LastOpenedFile"] == null) Settings.Default.LastOpenedFile = "";
            if (Settings.Default["LastOpenedAthleteId"] == null) Settings.Default.LastOpenedAthleteId = -1;
        }
    }
}
