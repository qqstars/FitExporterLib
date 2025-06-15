using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporterLib.DataModel.Types
{
    public class PositionType
    {
        /// <summary>
        /// Latitude (degree)
        /// </summary>
        public double LatitudeDegrees { get; set; }

        /// <summary>
        ///  Longitude (degree)
        /// </summary>
        public double LongitudeDegrees { get; set; }

        /// <summary>
        /// Altitude (Hight) (m)
        /// </summary>
        public double AltitudeMeters { get; set; }

        /// <summary>
        /// Delta Altitude (Hight changed) (m)
        /// </summary>
        public double DeltaAltitudeMeters { get; set; }

        /// <summary>
        /// Delta distance (m)
        /// </summary>
        public double DeltaDistanceMeters { get; set; }

        private double momentSlope = double.NaN;
        public double MomentSlope
        {
            get
            {
                if (double.IsNaN(momentSlope))
                {
                    double hMeter = Math.Sqrt(Math.Pow(this.DeltaDistanceMeters, 2) - Math.Pow(this.DeltaAltitudeMeters, 2));
                    double slope = !double.IsNaN(this.DeltaAltitudeMeters) && this.DeltaDistanceMeters > (double)1.0
                        ? this.DeltaAltitudeMeters / hMeter * 100 : (double)0;

                    slope = double.IsNaN(slope) ? 0 : slope;

                    return slope;
                }
                else
                {
                    return momentSlope;
                }
            }
            set
            {
                momentSlope = value;
            }
        }

        public PositionType()
        {
            this.LatitudeDegrees = double.NaN;
            this.LongitudeDegrees = double.NaN;
        }
    }
}
