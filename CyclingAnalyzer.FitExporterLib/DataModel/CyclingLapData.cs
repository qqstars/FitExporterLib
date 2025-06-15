using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporterLib.DataModel
{
    /// <summary>
    /// Lap data
    /// </summary>
    public class CyclingLapData
    {
        private CyclingData _Parent;
        private int lastCount = -1;
        private double lastTick = -65535;

        public CyclingLapData(ref CyclingData parent, DateTime startTime)
        {
            _Parent = parent;
            StartTime = startTime;

            this.TimeStandingSeconds = 0;
            this.StandCount = 0;
        }

        public DateTime StartTime { get; set; }

        private CyclingData lapData;

        public CyclingData LapData
        {
            get
            {
                if (null != _Parent && _Parent.DataPoints.Count > 0)
                {
                    if (lapData == null ||
                        (_Parent.DataPoints.Count != this.lastCount && _Parent.lastDataPointsUpdateTicks != lastTick))
                    {
                        DateTime endTime = this.StartTime;
                        DateTime startTime = this.StartTime;
                        if (_Parent.DataPoints.Count > 0 && startTime > _Parent.DataPoints.First().Time)
                        {
                            startTime = _Parent.DataPoints.First().Time;
                        }

                        for (int i = 0; i < _Parent.Laps.Count; i++)
                        {
                            if (i == _Parent.Laps.Count - 1)
                            { // last lap
                                endTime = _Parent.DataPoints.Last().Time.AddSeconds(1);
                                break;
                            }
                            else
                            {
                                if (_Parent.Laps[i].StartTime >= startTime)
                                {
                                    endTime = _Parent.Laps[i + 1].StartTime;
                                    break;
                                }
                            }
                        }

                        List<CyclingDataPoint> lst = new List<CyclingDataPoint>();

                        foreach (CyclingDataPoint cdp in _Parent.DataPoints)
                        {
                            if (cdp.Time >= startTime && cdp.Time < endTime)
                            {
                                lst.Add(cdp);
                            }
                            else if (cdp.Time >= endTime)
                            {
                                break;
                            }
                        }

                        this.lapData = new CyclingData(lst.ToArray(), _Parent);

                        this.lastCount = _Parent.DataPoints.Count;
                        this.lastTick = _Parent.lastDataPointsUpdateTicks;
                    }

                    return this.lapData;
                }
                else
                {
                    return new CyclingData();
                }
            }
        }

        public double Calories { get; set; }

        public double TimeStandingSeconds { get; set; }

        public TimeSpan TimeStanding
        {
            get
            {
                DateTime dt = new DateTime();
                dt = dt.AddSeconds(TimeStandingSeconds);
                return dt.Subtract(new DateTime());
            }
        }

        /// <summary>
        /// 85 means 85%
        /// </summary>
        public double StandingPercentage
        {
            get
            {
                if (lapData != null && lapData.SummaryTotalMoveTime.TotalSeconds > 0)
                {
                    return TimeStandingSeconds / lapData.SummaryTotalMoveTime.TotalSeconds * 100;
                }

                return 0;
            }
        }

        public int StandCount { get; set; }
    }
}
