using System;
using System.Collections.Generic;
using System.Text;

namespace OSL.Common.Model.ECharts
{
    public class DataZoomModel
    {
        public double startPercent;
        public double endPercent;
        /// <summary>
        /// datazoom start in epoch format
        /// </summary>
        public long startTime;
        /// <summary>
        /// datazoom start in epoch format
        /// </summary>
        public long endTime;
    }
}
