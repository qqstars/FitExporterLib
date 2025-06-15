using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporterLib
{
    public static class GlobalSettings
    {
        /// <summary>
        /// Gets or sets if calculate with Zero data value, set to true to include the zero data if stopped or not moving.
        /// </summary>
        public static bool IsCalculateWithZero { get; set; } = true;

        /// <summary>
        /// Gets or sets the max time second different between two point. Typically, when stopped, the device will stop recording and will cause two points to have a large time difference. Set this value to shorter the difference, for better calculation of average power, speed, etc.
        /// </summary>
        public static double MaxTimeSecondDifferentBetweenTwoPoint { get; set; } = 120;

        /// <summary>
        /// Gets or sets the minimum move speed (km/h) to consider the point as moving. Default is 0, which means all points are considered moving.
        /// </summary>
        public static double MinMoveSpeed { get; set; } = 0;

        /// <summary>
        /// Gets or set the default FTP (Functional Threshold Power) value. This is used for calculating power zones.
        /// </summary>
        public static double FTP { get; set; } = 200;

        /// <summary>
        /// Gets or sets the default LTHR (Lactate Threshold Heart Rate) value. This is used for calculating heart rate zones.
        /// </summary>
        public static double LTHR { get; set; } = 160;
    }
}
