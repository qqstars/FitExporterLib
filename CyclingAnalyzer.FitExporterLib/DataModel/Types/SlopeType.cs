using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporterLib.DataModel.Types
{
    public class SlopeType
    {
        /// <summary>
        /// Beginning point
        /// </summary>
        public CyclingDataPoint BeginningPoint
        {
            get { if (0 < this.DataPoints.Count) { return this.DataPoints[0]; } else { return null; }; }
        }

        /// <summary>
        /// Ending point
        /// </summary>
        public CyclingDataPoint EndingPoint
        {
            get { if (0 < this.DataPoints.Count) { return this.DataPoints[this.DataPoints.Count - 1]; } else { return null; }; }
        }

        private List<CyclingDataPoint> _DataPoints = new List<CyclingDataPoint>();

        /// <summary>
        /// DataPoints in this slope cycle
        /// </summary>
        public List<CyclingDataPoint> DataPoints
        {
            get { return _DataPoints; }
            set { _DataPoints = value; }
        }

        public double Distance
        {
            get
            {
                return this.EndingPoint.DistanceMeter - this.BeginningPoint.DistanceMeter;
            }
        }

        public double AltitudeChanged
        {
            get
            {
                return this.EndingPoint.Position.AltitudeMeters - this.BeginningPoint.Position.AltitudeMeters;
            }
        }

        public double MaxAltitude
        {
            get
            {
                if (this.DataPoints.Count > 1)
                {
                    return
                        this.DataPoints.Max((e) => { return e.Position.AltitudeMeters; });
                }
                else
                {
                    return 0;
                }
            }
        }

        public double MinAltitude
        {
            get
            {
                if (this.DataPoints.Count > 1)
                {
                    return
                        this.DataPoints.Min((e) => { return e.Position.AltitudeMeters; });
                }
                else
                {
                    return 0;
                }
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return EndingPoint.Time.Subtract(BeginningPoint.Time);
            }
        }

        /// <summary>
        /// Slope data. 8 means 8%
        /// </summary>
        public double Slope
        {
            get
            {
                if (this.DataPoints.Count <= 0)
                {
                    return 0;
                }
                else if (this.DataPoints.Count == 1)
                {
                    return this.DataPoints.First().Position.MomentSlope;
                }
                else
                {
                    CyclingDataPoint beginPoint = this.BeginningPoint;
                    CyclingDataPoint endPoint = this.EndingPoint;

                    double hMeter = Math.Sqrt(Math.Pow(endPoint.DistanceMeter - beginPoint.DistanceMeter, 2) - Math.Pow(endPoint.Position.AltitudeMeters - beginPoint.Position.AltitudeMeters, 2));
                    double slope = (endPoint.Position.AltitudeMeters - beginPoint.Position.AltitudeMeters)
                            / hMeter * 100;

                    if (slope > MaxMomentSlope || slope < MinMomentSlope)
                    {
                        slope = DataPoints.Sum(e => { return e.Position.MomentSlope; }) / DataPoints.Count;
                    }

                    return slope;
                }
            }
        }

        /// <summary>
        /// Max moment slope
        /// </summary>
        public double MaxMomentSlope
        {
            get
            {
                double slope = DataPoints.Max<CyclingDataPoint>((a) =>
                {
                    return !double.IsNaN(a.Position.MomentSlope) ? a.Position.MomentSlope : double.MinValue;
                });

                slope = slope > double.MinValue ? slope : 0;

                return slope;
            }
        }

        /// <summary>
        /// Min moment slope
        /// </summary>
        public double MinMomentSlope
        {
            get
            {
                var slope = DataPoints.Min<CyclingDataPoint>((a) =>
                {
                    return !double.IsNaN(a.Position.MomentSlope) ? a.Position.MomentSlope : double.MaxValue;
                });

                slope = slope < double.MaxValue ? slope : 0;

                return slope;
            }
        }

        public SlopeType(CyclingDataPoint[] points)
        {
            this.DataPoints.Clear();
            this.DataPoints.AddRange(points);
        }
    }
}
