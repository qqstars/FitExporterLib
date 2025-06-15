using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyclingAnalyzer.FitExporterLib.DataModel.Types;

namespace CyclingAnalyzer.FitExporterLib.DataModel
{
    /// <summary>
    /// Point data
    /// </summary>
    public class CyclingDataPoint
    {
        /// <summary>
        /// Recorded Time
        /// </summary>
        public DateTime Time { get; set; }

        public TimeSpan FixedTotalEllapsedForTimeBasedXData { get; set; }

        public double FixedTotalDistanceForDistancedXData { get; set; }

        private TimeSpan ts = new TimeSpan(1985);
        /// <summary>
        /// Elapsed Time
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get
            {
                if (ts.Ticks == 1985)
                {
                    ts = new TimeSpan(0, 0, 1);
                }

                return ts;
            }
            set
            {
                ts = value;
            }
        }

        private PositionType _Position;

        /// <summary>
        /// Position data
        /// </summary>
        public PositionType Position
        {
            get { if (null == _Position) { _Position = new PositionType(); } return _Position; }
            set { _Position = value; }
        }

        private double _DistanceMeter = 0;

        /// <summary>
        /// Total Distance meter (m)
        /// </summary>
        public double DistanceMeter
        {
            get { if (_DistanceMeter < 0) _DistanceMeter = 0; return _DistanceMeter; }
            set { _DistanceMeter = value; }
        }

        private double _HeartRateBpm = 0;
        /// <summary>
        /// Heart rate (bpm)
        /// </summary>
        public double HeartRateBpm
        {
            get { if (_HeartRateBpm < 0) _HeartRateBpm = 0; return _HeartRateBpm; }
            set { _HeartRateBpm = value > 250 ? 0 : value; }
        }

        private double _Cadence;
        /// <summary>
        /// Cadence data (cpm)
        /// </summary>
        public double Cadence
        {
            get { if (_Cadence < 0) _Cadence = 0; return _Cadence; }
            set { _Cadence = value; }
        }

        private PowerType _Power;

        /// <summary>
        /// Position data
        /// </summary>
        public PowerType Power
        {
            get { if (null == _Power) { _Power = new PowerType(); } return _Power; }
            set { _Power = value; }
        }

        private double _Speed = -1;
        /// <summary>
        /// Current speed (km/h)
        /// </summary>
        public double Speed
        {
            get { return _Speed; }//if (_Speed < 0) _Speed = _Speed;
            set { _Speed = value; }
        }

        /// <summary>
        /// Temperature (C degree)
        /// </summary>
        public double Temperature { get; set; }
    }

    public class StandEvent
    {
        public System.DateTime StartTime;
        public System.DateTime EndTime;
    }
}
