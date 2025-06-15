#define NET35

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using CyclingAnalyzer.FitExporterLib.DataModel.Types;

namespace CyclingAnalyzer.FitExporterLib.DataModel
{
    public class CyclingData
    {
        public static int DefaultParamSlopeSplitMinSeconds = 10;
        public static int DefaultParamSlopeSplitMeter = -1;

        public List<StandEvent> StandEvent = new List<StandEvent>();

        private bool IsCalculateWithZero { get; set; } = true;
        private double MaxTimeSecondDifferentBetweenTwoPoint { get; set; } = 5;
        private double MinMoveSpeed { get; set; } = 0;
        private double FTP { get; set; } = 200;
        private double LTHR { get; set; } = 160;

        #region constructor
        public CyclingData()
        {

        }

        public CyclingData(CyclingDataPoint[] dataPoints, CyclingData sourceData = null)
        {
            int paramSlopeSplitMinSeconds = DefaultParamSlopeSplitMinSeconds;
            int paramSlopeSplitMeter = DefaultParamSlopeSplitMeter;

            this.DataPoints.AddRange(dataPoints);

            if (null != sourceData)
            {
                paramSlopeSplitMeter = sourceData.ParamSlopeSplitMeter;
                paramSlopeSplitMinSeconds = sourceData.ParamSlopeSplitMinSeconds;

                CyclingLapData lastLap = null;

                if (this.DataPoints.Count > 0)
                {
                    DateTime lastTime = this.DataPoints.Last().Time;
                    DateTime startTime = this.DataPoints.First().Time;
                    foreach (var lap in sourceData.Laps)
                    {
                        if (lap.StartTime < lastTime && lap.StartTime >= startTime)
                        {
                            if (0 >= this.Laps.Count && lastLap != null && lap.StartTime != this.DataPoints.First().Time)
                            {
                                this.Laps.Add(lastLap);
                            }

                            this.Laps.Add(lap);
                        }
                        else if (0 >= this.Laps.Count && lap.StartTime >= lastTime)
                        {
                            this.Laps.Add(lastLap);
                            break;
                        }

                        lastLap = lap;
                    }
                }
            }

            this._SlopeSplitSeconds = paramSlopeSplitMinSeconds;
            this._SlopeSplitMeter = paramSlopeSplitMeter;

            this.GetExtraData(paramSlopeSplitMeter);

            this.lastDataPointsCount = -1;
            this.UpdateSummaryData();
        }

        public CyclingData(CyclingDataPoint[] dataPoints, int paramSlopeSplitMeter, int paramSlopeSplitMinSeconds)
        {
            this.DataPoints.AddRange(dataPoints);
            this._SlopeSplitSeconds = paramSlopeSplitMinSeconds;
            this._SlopeSplitMeter = paramSlopeSplitMeter;

            this.GetExtraData(paramSlopeSplitMeter);

            this.lastDataPointsCount = -1;
            this.UpdateSummaryData();
        }
        #endregion

        #region Add CyclingDataPoint with fixing
        public void AddCyclingDataPointWithFixing(CyclingDataPoint point)
        {
            #region data fixing
            if (this._DataPoints.Count <= 0)
            {
                point.ElapsedTime = new TimeSpan(0);

                point.Position.DeltaAltitudeMeters = 0;
                point.Position.DeltaDistanceMeters = 0;

                if (point.Speed < 0)
                {
                    point.Speed = 0;
                }

                if (255 <= point.Cadence || double.IsNaN(point.Cadence))
                {
                    point.Cadence = 0;
                }

                if (65535 <= point.Power.LeftLegPower || double.IsNaN(point.Power.LeftLegPower))
                {
                    point.Power.LeftLegPower = 0;
                }

                if (65535 <= point.Power.RightLegPower || double.IsNaN(point.Power.RightLegPower))
                {
                    point.Power.RightLegPower = 0;
                }

                if (65535 <= point.Power.TotalPower || double.IsNaN(point.Power.TotalPower))
                {
                    point.Power.TotalPower = 0;
                }

                point.FixedTotalEllapsedForTimeBasedXData = new TimeSpan(0);
                point.FixedTotalDistanceForDistancedXData = 0;
            }
            else
            {
                var lastPoint = this._DataPoints.Last();
                DateTime lastStopTimeGet = this._DataPoints.Where((p) => { return p.DistanceMeter >= lastPoint.DistanceMeter; }).First().Time;

                var lastSpeedObj = this._DataPoints.LastOrDefault((p) => { return p.Speed > 0; });
                double lastSpeedGet = null != lastSpeedObj ? lastSpeedObj.Speed : 0;
                lastSpeedGet = lastSpeedGet > 0 ? lastSpeedGet : 0;

                point.ElapsedTime = point.Time.Subtract(lastPoint.Time);

                point.Position.DeltaAltitudeMeters = point.Position.AltitudeMeters - lastPoint.Position.AltitudeMeters;
                point.Position.DeltaDistanceMeters = point.DistanceMeter - lastPoint.DistanceMeter;


                if (point.Speed < 0)//&& point.ElapsedTime.TotalSeconds > 0
                {
                    if (point.Position.DeltaDistanceMeters > 0)
                    {
                        if (lastStopTimeGet == lastPoint.Time)
                        {
                            point.Speed = point.Position.DeltaDistanceMeters / point.Time.Subtract(lastStopTimeGet).TotalSeconds * 3.6;//point.ElapsedTime.TotalSeconds * 3.6;
                        }
                        else
                        {
                            point.Speed = lastSpeedGet;
                        }
                    }
                    else
                    {
                        point.Speed = 0;
                    }
                }

                if (this.MaxTimeSecondDifferentBetweenTwoPoint < point.ElapsedTime.TotalSeconds)
                {
                    if (point.Speed <= 1)
                    {
                        point.ElapsedTime = new TimeSpan(0, 0, 1);
                    }
                    else
                    {
                        double elepsed = point.ElapsedTime.TotalSeconds;
                        elepsed = Math.Min(elepsed, point.Position.DeltaDistanceMeters / point.Speed * 3.6);
                        point.ElapsedTime = new System.DateTime().AddSeconds(
                            elepsed)
                            .Subtract(new System.DateTime());
                    }
                }


                if (point.DistanceMeter <= 0)
                { // distance meter data not contained
                    point.Position.DeltaDistanceMeters = point.Speed / 3.6 * point.ElapsedTime.TotalSeconds;
                    point.DistanceMeter = lastPoint.DistanceMeter + point.Position.DeltaDistanceMeters;
                }

                if (point.Position.DeltaDistanceMeters <= 0)
                { // after all fixing, delta distance is 0, means altitude will NOT changed even if the real alti diffenrent is not 0
                    point.Position.DeltaAltitudeMeters = 0;
                }

                double curSlp = point.Position.MomentSlope;

                if (2 <= this._DataPoints.Count &&
                    !double.IsNaN(lastPoint.Position.MomentSlope)
                    && (curSlp > 30 || curSlp < -30)
                    && Math.Abs(curSlp - lastPoint.Position.MomentSlope) > 10)
                {
                    point.Position.MomentSlope = curSlp < lastPoint.Position.MomentSlope ? lastPoint.Position.MomentSlope - 10 : lastPoint.Position.MomentSlope + 10;
                }

                if (255 <= point.Cadence || double.IsNaN(point.Cadence))
                {
                    point.Cadence = lastPoint.Cadence;
                }

                if (65535 <= point.Power.LeftLegPower || double.IsNaN(point.Power.LeftLegPower))
                {
                    point.Power.LeftLegPower = 0;
                }

                if (65535 <= point.Power.RightLegPower || double.IsNaN(point.Power.RightLegPower))
                {
                    point.Power.RightLegPower = 0;
                }

                if (65535 <= point.Power.TotalPower || double.IsNaN(point.Power.TotalPower))
                {
                    point.Power.TotalPower = 0;
                }

                #region fixed time based X
                ////DateTime dt = point.Time;
                ////if (lastPoint.ElapsedTime != null && lastPoint.Time.Subtract(new DateTime()).TotalSeconds > 10000
                ////    && dt.Subtract(lastPoint.Time).TotalSeconds > StaticResources.MaxTimeSecondDifferentBetweenTwoPoint)//isCheckPointAvaliable && 
                ////{
                point.FixedTotalEllapsedForTimeBasedXData = lastPoint.FixedTotalEllapsedForTimeBasedXData.Add(point.ElapsedTime);
                ////}
                ////else
                ////{
                ////    point.FixedTotalEllapsedForTimeBasedXData = lastPoint.FixedTotalEllapsedForTimeBasedXData.Add(dt.Subtract(lastPoint.Time));
                ////}

                point.FixedTotalDistanceForDistancedXData = lastPoint.FixedTotalDistanceForDistancedXData + point.Position.DeltaDistanceMeters;

                #endregion
            }

            #endregion

            bool isStand = false;

            foreach (var e in this.StandEvent)
            {
                if (point.Time >= e.StartTime && point.Time <= e.EndTime)
                {
                    isStand = true;
                    break;
                }
            }

            if (isStand)
            {
                point.Power.StanceTime = point.ElapsedTime.TotalMilliseconds;
            }

            this._DataPoints.Add(point);
        }

        public void ProcessStanding(StandEvent newEvent)
        {
            if (this.DataPoints == null)
            {
                return;
            }

            foreach (var p in this.DataPoints)
            {
                if (p.Time > newEvent.EndTime)
                {
                    break;
                }

                if (p.Time >= newEvent.StartTime && p.Time <= newEvent.EndTime)
                {
                    p.Power.StanceTime = p.ElapsedTime.TotalMilliseconds;
                }
            }
        }

        public void ProcessAllStanding()
        {
            if (this.DataPoints == null)
            {
                return;
            }

            if (this.StandEvent.Count <= 0)
            {
                return;
            }

            var sorted = this.StandEvent.OrderBy<StandEvent, DateTime>((p) => { return p.StartTime; }).ToArray();

            var index = 0;
            var currentFirst = sorted[0];

            foreach (var p in this.DataPoints)
            {
                if (p.Time >= currentFirst.StartTime && p.Time <= currentFirst.EndTime)
                {
                    p.Power.StanceTime = p.ElapsedTime.TotalMilliseconds;
                }

                if (p.Time >= currentFirst.EndTime)
                {
                    index++;
                    if (sorted.Count() <= index)
                    {
                        break;
                    }

                    currentFirst = sorted[index];
                }
            }
        }
        #endregion

        #region Range Data
        public PowerRange PowerRangeData;

        public HeartRateRange HeartRateRangeData;
        #endregion

        #region properties
        private List<CyclingLapData> _Laps = new List<CyclingLapData>();

        /// <summary>
        /// Laps data
        /// </summary>
        public List<CyclingLapData> Laps
        {
            get { return _Laps; }
        }

        private List<CyclingDataPoint> _DataPoints = new List<CyclingDataPoint>();
        bool isReordered = false;
        public double lastDataPointsUpdateTicks = -1;
        /// <summary>
        /// Data points from record
        /// </summary>
        public List<CyclingDataPoint> DataPoints
        {
            get
            {
                if (!isReordered && _DataPoints != null && _DataPoints.Count > 0)
                {
                    var points = from p in _DataPoints orderby p.Time select p;
                    _DataPoints = new List<CyclingDataPoint>(points);
                    isReordered = true;
                }
                return _DataPoints;
            }
            set { isReordered = false; _DataPoints = value; lastDataPointsUpdateTicks = DateTime.Now.Ticks; this.lastDataPointsCount = -1; this.UpdateSummaryData(); }
        }

        private List<SlopeType> _SlopeData = new List<SlopeType>();

        /// <summary>
        /// Slope data list
        /// </summary>
        public List<SlopeType> SlopeData
        {
            get { return _SlopeData; }
            set { _SlopeData = value; }
        }

        #endregion

        #region parameter
        private int _SlopeSplitMeter = DefaultParamSlopeSplitMeter;

        /// <summary>
        /// Slope split meter. If set 500, means calculate slope for every 500m.
        /// </summary>
        public int ParamSlopeSplitMeter
        {
            get { return _SlopeSplitMeter; }
            set { if (_SlopeSplitMeter != value) { this.GetSlopeData(value); } _SlopeSplitMeter = value; }
        }

        private int _SlopeSplitSeconds = DefaultParamSlopeSplitMinSeconds;

        /// <summary>
        /// Slope split meter. If set 10, means if 10s slope changed very much, it will record the change.
        /// </summary>
        public int ParamSlopeSplitMinSeconds
        {
            get { return _SlopeSplitSeconds; }
            set { if (_SlopeSplitSeconds != value) { _SlopeSplitSeconds = value; this.GetSlopeData(this._SlopeSplitMeter); } _SlopeSplitSeconds = value; }
        }

        #endregion

        #region Summary
        private int lastDataPointsCount = 0;
        private bool? isCalculateZeroLast = null;

        private double _SummaryTotalDistance;
        TimeSpan _SummaryTotalMoveTime;
        double _SummaryTotalAltitudeGain;
        double _SummaryTotalAltitudeLost;
        double _SummaryAvgTemperature;
        double _SummaryAvgHeartRate;
        double _SummaryAvgCadence;
        double _SummaryAvgPower;
        double _SummaryAvgAccumulatedPower;
        double _SummaryAvgLeftLegPower;
        double _SummaryAvgRightLegPower;
        double _SummaryAvgLeftTorqueEffectiveness;
        double _SummaryAvgLeftPedalSmoothness;
        double _SummaryAvgRightTorqueEffectiveness;
        double _SummaryAvgRightPedalSmoothness;
        double _SummaryAvgTorqueEffectiveness;
        double _SummaryAvgPedalSmoothness;
        double _SummaryLeftPowerPhaseStart = double.NaN;
        double _SummaryLeftPowerPhaseEnd = double.NaN;
        double _SummaryLeftPowerPhasePeakStart = double.NaN;
        double _SummaryLeftPowerPhasePeakEnd = double.NaN;
        double _SummaryLeftPlatformCenterOffset = double.NaN;
        double _SummaryRightPowerPhaseStart = double.NaN;
        double _SummaryRightPowerPhaseEnd = double.NaN;
        double _SummaryRightPowerPhasePeakStart = double.NaN;
        double _SummaryRightPowerPhasePeakEnd = double.NaN;
        double _SummaryRightPlatformCenterOffset = double.NaN;
        TimeSpan _SummaryTotalStandTime;
        int _SummaryTotalStandCount;
        bool isCalculateStand = true;
        bool isCalculateSeat = true;

        public void UpdateSummaryData()
        {
            if (this._DataPoints == null)
            {
                return;
            }

            if (this._DataPoints.Count == lastDataPointsCount && isCalculateZeroLast.HasValue && isCalculateZeroLast.Value == this.IsCalculateWithZero)
            {
                return;
            }

            double totalSeconds = 0;
            double totalHRS = 0;
            double totalCadS = 0;
            double totalPWS = 0;
            double totalTES = 0;

            double totalLeftDMS = 0;
            double totalRightDMS = 0;
            double totalLeftPCOS = 0;
            double totalRightPCOS = 0;

            bool isLastSeat = true;
            #region init
            this._SummaryTotalDistance = 0;
            this._SummaryTotalMoveTime = new TimeSpan(0);
            this._SummaryTotalAltitudeGain = 0;
            this._SummaryTotalAltitudeLost = 0;
            this._SummaryAvgTemperature = 0;
            this._SummaryAvgHeartRate = 0;
            this._SummaryAvgCadence = 0;
            this._SummaryAvgPower = 0;
            this._SummaryAvgAccumulatedPower = 0;
            this._SummaryAvgLeftLegPower = 0;
            this._SummaryAvgLeftTorqueEffectiveness = 0;
            this._SummaryAvgLeftPedalSmoothness = 0;
            this._SummaryAvgRightLegPower = 0;
            this._SummaryAvgRightTorqueEffectiveness = 0;
            this._SummaryAvgRightPedalSmoothness = 0;
            this._SummaryAvgTorqueEffectiveness = 0;
            this._SummaryAvgPedalSmoothness = 0;

            this._SummaryMaxAltitude = double.MinValue;
            this._SummaryMaxCadence = 0;
            this._SummaryMaxHeartRate = 0;
            this._SummaryMaxLeftLegPower = 0;
            this._SummaryMaxMomentSlope = double.MinValue;
            this._SummaryMaxPower = 0;
            this._SummaryMaxRightLegPower = 0;
            this._SummaryMaxSpeed = 0;
            this._SummaryMaxTemperature = double.MinValue;
            this._SummaryMinAltitude = double.MaxValue;
            this._SummaryMinCadence = 0;
            this._SummaryMinHeartRate = 0;
            this._SummaryMinMomentSlope = double.MaxValue;
            this._SummaryMinTemperature = double.MaxValue;

            this._SummaryLeftPlatformCenterOffset = 0;
            this._SummaryLeftPowerPhaseEnd = 0;
            this._SummaryLeftPowerPhasePeakEnd = 0;
            this._SummaryLeftPowerPhasePeakStart = 0;
            this._SummaryLeftPowerPhaseStart = 0;
            this._SummaryRightPlatformCenterOffset = 0;
            this._SummaryRightPowerPhaseEnd = 0;
            this._SummaryRightPowerPhasePeakEnd = 0;
            this._SummaryRightPowerPhasePeakStart = 0;
            this._SummaryRightPowerPhaseStart = 0;

            this._SummaryTotalStandCount = 0;
            this._SummaryTotalStandTime = new TimeSpan();
            #endregion

            #region loop
            var loopCount = this._DataPoints.Count > 2 ? this._DataPoints.Count : 1;
            ////if (this._DataPoints.Count <= 0)
            ////{
            ////    loopCount = 0;
            ////}

            for (int i = 0; i < loopCount; i++)
            {
                var e = this._DataPoints[i];

                if (!this.isCalculateSeat && !e.Power.IsStand)
                {
                    continue;
                }
                else if (!this.isCalculateStand && e.Power.IsStand)
                {
                    continue;
                }

                try
                {
                    var seconds = e.ElapsedTime.TotalSeconds;
                    totalSeconds += seconds;

                    this._SummaryTotalDistance += !double.IsNaN(e.Position.DeltaDistanceMeters) ? e.Position.DeltaDistanceMeters : 0;
                    this._SummaryTotalAltitudeGain += e.Position.DeltaAltitudeMeters > 0 ? e.Position.DeltaAltitudeMeters : 0;
                    this._SummaryTotalAltitudeLost += e.Position.DeltaAltitudeMeters < 0 ? e.Position.DeltaAltitudeMeters * -1 : 0;
                    this._SummaryAvgTemperature += e.Temperature * seconds;
                    this._SummaryAvgHeartRate += e.HeartRateBpm * seconds;
                    totalHRS += e.HeartRateBpm > 0 || this.IsCalculateWithZero ? seconds : 0;
                    this._SummaryAvgCadence += e.Cadence * seconds;
                    totalCadS += e.Cadence > 0 || this.IsCalculateWithZero ? seconds : 0;
                    this._SummaryAvgPower += e.Power.TotalPower * seconds;
                    this._SummaryAvgAccumulatedPower += e.Power.AccumulatedPower * seconds;
                    this._SummaryAvgLeftLegPower += e.Power.LeftLegPower * seconds;
                    this._SummaryAvgLeftTorqueEffectiveness += e.Power.LeftTorqueEffectiveness * seconds;
                    this._SummaryAvgLeftPedalSmoothness += e.Power.LeftPedalSmoothness * seconds;
                    this._SummaryAvgRightLegPower += e.Power.RightLegPower * seconds;
                    this._SummaryAvgRightTorqueEffectiveness += e.Power.RightTorqueEffectiveness * seconds;
                    this._SummaryAvgRightPedalSmoothness += e.Power.RightPedalSmoothness * seconds;
                    this._SummaryAvgTorqueEffectiveness += e.Power.TorqueEffectiveness * seconds;
                    this._SummaryAvgPedalSmoothness += e.Power.PedalSmoothness * seconds;
                    totalPWS += e.Power.TotalPower > 0 || this.IsCalculateWithZero ? seconds : 0;
                    totalTES += e.Power.TotalPower > 0 ? seconds : 0;

                    if (!double.IsNaN(e.Power.LeftPlatformCenterOffset)
                        && !double.IsNaN(e.Power.LeftPowerPhaseStart) && !double.IsNaN(e.Power.LeftPowerPhaseEnd))
                    {
                        this._SummaryLeftPlatformCenterOffset += e.Power.LeftPlatformCenterOffset * seconds;

                        totalLeftPCOS += seconds;
                    }

                    if (!double.IsNaN(e.Power.LeftPowerPhaseEnd) && !double.IsNaN(e.Power.LeftPowerPhasePeakEnd)
                        && !double.IsNaN(e.Power.LeftPowerPhasePeakStart) && !double.IsNaN(e.Power.LeftPowerPhaseStart))
                    {
                        this._SummaryLeftPowerPhaseEnd += e.Power.LeftPowerPhaseEnd * seconds;
                        this._SummaryLeftPowerPhasePeakEnd += e.Power.LeftPowerPhasePeakEnd * seconds;
                        this._SummaryLeftPowerPhasePeakStart += this.GetSummaryAngle(e.Power.LeftPowerPhasePeakStart) * seconds;
                        this._SummaryLeftPowerPhaseStart += this.GetSummaryAngle(e.Power.LeftPowerPhaseStart) * seconds;

                        totalLeftDMS += seconds;
                    }

                    if (!double.IsNaN(e.Power.RightPlatformCenterOffset)
                        && !double.IsNaN(e.Power.RightPowerPhaseStart) && !double.IsNaN(e.Power.RightPowerPhaseEnd))
                    {
                        this._SummaryRightPlatformCenterOffset += e.Power.RightPlatformCenterOffset * seconds;

                        totalRightPCOS += seconds;
                    }

                    if (!double.IsNaN(e.Power.RightPowerPhaseEnd) && !double.IsNaN(e.Power.RightPowerPhasePeakEnd)
                        && !double.IsNaN(e.Power.RightPowerPhasePeakStart) && !double.IsNaN(e.Power.RightPowerPhaseStart))
                    {
                        this._SummaryRightPowerPhaseEnd += e.Power.RightPowerPhaseEnd * seconds;
                        this._SummaryRightPowerPhasePeakEnd += e.Power.RightPowerPhasePeakEnd * seconds;
                        this._SummaryRightPowerPhasePeakStart += this.GetSummaryAngle(e.Power.RightPowerPhasePeakStart) * seconds;
                        this._SummaryRightPowerPhaseStart += this.GetSummaryAngle(e.Power.RightPowerPhaseStart) * seconds;

                        totalRightDMS += seconds;
                    }

                    this._SummaryMaxAltitude = Math.Max(this._SummaryMaxAltitude, e.Position.AltitudeMeters);
                    this._SummaryMaxCadence = Math.Max(this._SummaryMaxCadence, e.Cadence);
                    this._SummaryMaxHeartRate = Math.Max(this._SummaryMaxHeartRate, e.HeartRateBpm);
                    this._SummaryMaxLeftLegPower = Math.Max(this._SummaryMaxLeftLegPower, e.Power.LeftLegPower);
                    this._SummaryMaxMomentSlope = Math.Max(this._SummaryMaxMomentSlope, !double.IsNaN(e.Position.MomentSlope) ? e.Position.MomentSlope : double.MinValue);
                    this._SummaryMaxPower = Math.Max(this._SummaryMaxPower, e.Power.TotalPower);
                    this._SummaryMaxRightLegPower = Math.Max(this._SummaryMaxRightLegPower, e.Power.RightLegPower);
                    this._SummaryMaxSpeed = Math.Max(this._SummaryMaxSpeed, e.Speed);
                    this._SummaryMaxTemperature = Math.Max(this._SummaryMaxTemperature, e.Temperature);
                    this._SummaryMinAltitude = Math.Min(this._SummaryMinAltitude, e.Position.AltitudeMeters);
                    this._SummaryMinCadence = Math.Min(this._SummaryMinCadence, e.Cadence);
                    this._SummaryMinHeartRate = Math.Min(this._SummaryMinHeartRate, e.HeartRateBpm);
                    this._SummaryMinMomentSlope = Math.Min(this._SummaryMinMomentSlope, !double.IsNaN(e.Position.MomentSlope) ? e.Position.MomentSlope : double.MaxValue);
                    this._SummaryMinTemperature = Math.Min(this._SummaryMinTemperature, e.Temperature);

                    if (e.Speed >= this.MinMoveSpeed || this._DataPoints.Count == 1)
                    {
                        this._SummaryTotalMoveTime = this._SummaryTotalMoveTime.Add(e.ElapsedTime);
                    }

                    this._SummaryTotalStandTime = this._SummaryTotalStandTime.Add(new TimeSpan(0, 0, 0, 0, (int)e.Power.StanceTime));

                    if (isLastSeat && e.Power.IsStand)
                    {
                        this._SummaryTotalStandCount++;
                        isLastSeat = false;
                    }
                    else if (!isLastSeat && !e.Power.IsStand)
                    {
                        isLastSeat = true;
                    }
                }
                catch { }
            }
            #endregion

            #region calculate avg
            this._SummaryAvgTemperature = totalSeconds > 0 ? this._SummaryAvgTemperature / totalSeconds : 0;
            this._SummaryAvgHeartRate = totalHRS > 0 ? this._SummaryAvgHeartRate / totalHRS : 0;
            this._SummaryAvgCadence = totalCadS > 0 ? this._SummaryAvgCadence / totalCadS : 0;
            this._SummaryAvgPower = totalPWS > 0 ? this._SummaryAvgPower / totalPWS : 0;
            this._SummaryAvgAccumulatedPower = totalPWS > 0 ? this._SummaryAvgAccumulatedPower / totalPWS : 0;
            this._SummaryAvgLeftLegPower = totalPWS > 0 ? this._SummaryAvgLeftLegPower / totalPWS : 0;
            this._SummaryAvgLeftTorqueEffectiveness = totalTES > 0 ? this._SummaryAvgLeftTorqueEffectiveness / totalTES : 0;
            this._SummaryAvgLeftPedalSmoothness = totalTES > 0 ? this._SummaryAvgLeftPedalSmoothness / totalTES : 0;
            this._SummaryAvgRightLegPower = totalPWS > 0 ? this._SummaryAvgRightLegPower / totalPWS : 0;
            this._SummaryAvgRightTorqueEffectiveness = totalTES > 0 ? this._SummaryAvgRightTorqueEffectiveness / totalTES : 0;
            this._SummaryAvgRightPedalSmoothness = totalTES > 0 ? this._SummaryAvgRightPedalSmoothness / totalTES : 0;
            this._SummaryAvgTorqueEffectiveness = totalTES > 0 ? this._SummaryAvgTorqueEffectiveness / totalTES : 0;
            this._SummaryAvgPedalSmoothness = totalTES > 0 ? this._SummaryAvgPedalSmoothness / totalTES : 0;

            this._SummaryLeftPlatformCenterOffset = totalLeftPCOS > 0 ? this._SummaryLeftPlatformCenterOffset / totalLeftPCOS : double.NaN;
            this._SummaryLeftPowerPhaseEnd = totalLeftDMS > 0 ? this._SummaryLeftPowerPhaseEnd / totalLeftDMS : double.NaN;
            this._SummaryLeftPowerPhasePeakEnd = totalLeftDMS > 0 ? this._SummaryLeftPowerPhasePeakEnd / totalLeftDMS : double.NaN;
            this._SummaryLeftPowerPhasePeakStart = totalLeftDMS > 0 ? this.GetFixedAngle(this._SummaryLeftPowerPhasePeakStart / totalLeftDMS) : double.NaN;
            this._SummaryLeftPowerPhaseStart = totalLeftDMS > 0 ? this.GetFixedAngle(this._SummaryLeftPowerPhaseStart / totalLeftDMS) : double.NaN;

            this._SummaryRightPlatformCenterOffset = totalRightPCOS > 0 ? this._SummaryRightPlatformCenterOffset / totalRightPCOS : double.NaN;
            this._SummaryRightPowerPhaseEnd = totalRightDMS > 0 ? this._SummaryRightPowerPhaseEnd / totalRightDMS : double.NaN;
            this._SummaryRightPowerPhasePeakEnd = totalRightDMS > 0 ? this._SummaryRightPowerPhasePeakEnd / totalRightDMS : double.NaN;
            this._SummaryRightPowerPhasePeakStart = totalRightDMS > 0 ? this.GetFixedAngle(this._SummaryRightPowerPhasePeakStart / totalRightDMS) : double.NaN;
            this._SummaryRightPowerPhaseStart = totalRightDMS > 0 ? this.GetFixedAngle(this._SummaryRightPowerPhaseStart / totalRightDMS) : double.NaN;
            #endregion

            if (this._DataPoints.Count > 2)
            {
                var lastP = this._DataPoints.Last();
                var firstP = this._DataPoints.First();
                if (!(double.IsNaN(lastP.Position.DeltaDistanceMeters) && lastP.ElapsedTime == new TimeSpan()))
                { // last point is NOT empty point
                    #region last Fix
                    this._SummaryTotalDistance -= firstP.Position.DeltaDistanceMeters;
                    this._SummaryTotalMoveTime.Subtract(firstP.ElapsedTime);
                    this._SummaryTotalAltitudeGain -= firstP.Position.DeltaAltitudeMeters > 0 ? firstP.Position.DeltaAltitudeMeters : 0;
                    this._SummaryTotalAltitudeLost -= firstP.Position.DeltaAltitudeMeters < 0 ? firstP.Position.DeltaAltitudeMeters * -1 : 0;
                    #endregion
                }
            }

            #region fix MaxMin
            this._SummaryMaxAltitude = this._SummaryMaxAltitude > double.MinValue ? this._SummaryMaxAltitude : 0;
            //this.SummaryMaxCadence = 0;
            //this.SummaryMaxHeartRate = 0;
            //this.SummaryMaxLeftLegPower = 0;
            this._SummaryMaxMomentSlope = this._SummaryMaxMomentSlope > double.MinValue ? this._SummaryMaxMomentSlope : 0;
            //this.SummaryMaxPower = 0;
            //this.SummaryMaxRightLegPower = 0;
            //this.SummaryMaxSpeed = 0;
            this._SummaryMaxTemperature = this._SummaryMaxTemperature > double.MinValue ? this._SummaryMaxTemperature : 0;
            this._SummaryMinAltitude = this._SummaryMinAltitude < double.MaxValue ? this._SummaryMinAltitude : 0;
            //this.SummaryMinCadence = 0;
            //this.SummaryMinHeartRate = 0;
            this._SummaryMinMomentSlope = this._SummaryMinMomentSlope < double.MaxValue ? this._SummaryMinMomentSlope : 0;
            this._SummaryMinTemperature = this._SummaryMinTemperature < double.MaxValue ? this._SummaryMinTemperature : 0; ;
            #endregion

            lastDataPointsCount = this._DataPoints.Count;
            isCalculateZeroLast = this.IsCalculateWithZero;
        }

        public void UpdateSummaryData(bool calculateStand, bool calculateSeat)
        {
            this.isCalculateStand = calculateStand;
            this.isCalculateSeat = calculateSeat;
            this.lastDataPointsCount = -1;

            this.UpdateSummaryData();
        }

        /// <summary>
        /// Re-calculate xBased data for distance based data or time based data.
        /// </summary>
        public void UpdateXBasedData()
        {
            if (this._DataPoints != null && this._DataPoints.Count > 0)
            {
                if (this._DataPoints.Last().ElapsedTime.TotalSeconds > 0
                    || (!double.IsNaN(this._DataPoints.Last().Position.DeltaDistanceMeters) && this._DataPoints.Last().Position.DeltaDistanceMeters > 0))
                {
                    var newP = this.GetMaxNPowerDataPoint(new CyclingDataPoint[] { this._DataPoints.Last() });
                    newP.ElapsedTime = new TimeSpan();
                    newP.Position.DeltaDistanceMeters = 0;
                    this._DataPoints.Add(newP);
                }

                for (int i = 0; i < this._DataPoints.Count; i++)
                {
                    var e = this._DataPoints[i];
                    if (i == 0)
                    {
                        e.FixedTotalDistanceForDistancedXData = 0;
                        e.FixedTotalEllapsedForTimeBasedXData = new TimeSpan(0);
                    }
                    else
                    {
                        var lastP = this._DataPoints[i - 1];

                        e.FixedTotalEllapsedForTimeBasedXData = lastP.FixedTotalEllapsedForTimeBasedXData.Add(lastP.ElapsedTime);
                        e.FixedTotalDistanceForDistancedXData = lastP.FixedTotalDistanceForDistancedXData +
                            (!double.IsNaN(lastP.Position.DeltaDistanceMeters) ? lastP.Position.DeltaDistanceMeters : 0);
                    }
                }
            }
        }

        public double SummaryTotalDistance
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryTotalDistance;
            }
        }

        public TimeSpan SummaryTotalTime
        {
            get
            {
                if (this.DataPoints.Count > 2)
                {
                    return this.DataPoints.Last().Time.Subtract(this.DataPoints.First().Time).Add(this.DataPoints.Last().ElapsedTime);
                }
                else if (this.DataPoints.Count == 2 || this.DataPoints.Count == 1)
                {
                    return this.DataPoints.First().ElapsedTime;
                }
                else
                {
                    return new TimeSpan(0);
                }
            }
        }

        public TimeSpan SummaryTotalMoveTime
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryTotalMoveTime;
            }
        }

        public double SummaryTotalAltitudeGain
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryTotalAltitudeGain;
            }
        }

        public double SummaryTotalAltitudeLost
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryTotalAltitudeLost;
            }
        }

        public double SummaryTotalCalories
        {
            get
            {
                if (this.Laps.Count > 0)
                {
                    return
                        this.Laps.Sum((e) =>
                        {
                            return e.Calories;
                        });
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SummaryAvgSlope
        {
            get
            {
                var vDistance = (this.SummaryTotalAltitudeGain - this.SummaryTotalAltitudeLost);
                var hDistance = Math.Pow(this.SummaryTotalDistance * this.SummaryTotalDistance - vDistance * vDistance, 0.5);
                return vDistance / hDistance * 100;
            }
        }

        /// <summary>
        /// km/h
        /// </summary>
        public double SummaryAvgSpeed
        {
            get
            {
                double totalDistance = this.SummaryTotalDistance;

                if (totalDistance > 0)
                {
                    return totalDistance / this.SummaryTotalTime.TotalMilliseconds * 1000 * 3.6;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// km/h
        /// </summary>
        public double SummaryAvgMoveSpeed
        {
            get
            {
                double totalDistance = this.SummaryTotalDistance;
                TimeSpan totalMoveTime = this.SummaryTotalMoveTime;
                if (totalDistance > 0 && totalMoveTime.TotalMilliseconds > 0)
                {
                    return totalDistance / totalMoveTime.TotalMilliseconds * 1000 * 3.6;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SummaryAvgTemperature
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgTemperature;
            }
        }

        public double SummaryAvgHeartRate
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgHeartRate;
            }
        }

        public double SummaryAvgCadence
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgCadence;
            }
        }

        public double SummaryAvgPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgPower;
            }
        }

        public double SummaryAvgAccumulatedPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgAccumulatedPower;
            }
        }

        public double SummaryAvgLeftLegPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgLeftLegPower;
            }
        }

        public double SummaryAvgRightLegPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgRightLegPower;
            }
        }

        public double SummaryAvgLeftLegPowerPercentage
        {
            get
            {
                if (this.SummaryAvgPower > 0)
                {
                    return this.SummaryAvgLeftLegPower / this.SummaryAvgPower * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SummaryAvgRightLegPowerPercentage
        {
            get
            {
                if (this.SummaryAvgPower > 0)
                {
                    return this.SummaryAvgRightLegPower / this.SummaryAvgPower * 100;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SummaryAvgLeftTorqueEffectiveness
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgLeftTorqueEffectiveness;
            }
        }

        public double SummaryAvgRightTorqueEffectiveness
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgRightTorqueEffectiveness;
            }
        }

        public double SummaryAvgTorqueEffectiveness
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgTorqueEffectiveness;
            }
        }

        public double SummaryAvgLeftPedalSmoothness
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgLeftPedalSmoothness;
            }
        }

        public double SummaryAvgRightPedalSmoothness
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgRightPedalSmoothness;
            }
        }

        public double SummaryAvgPedalSmoothness
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryAvgPedalSmoothness;
            }
        }

        double _SummaryMaxSpeed;
        public double SummaryMaxSpeed
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxSpeed;
            }
            private set
            {
                this._SummaryMaxSpeed = value;
            }
        }

        double _SummaryMaxHeartRate;
        public double SummaryMaxHeartRate
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxHeartRate;
            }
            private set
            {
                this._SummaryMaxHeartRate = value;
            }
        }

        double _SummaryMinHeartRate;
        public double SummaryMinHeartRate
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMinHeartRate;
            }
            private set
            {
                this._SummaryMinHeartRate = value;
            }
        }

        double _SummaryMaxCadence;
        public double SummaryMaxCadence
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxCadence;
            }
            private set
            {
                this._SummaryMaxCadence = value;
            }
        }

        double _SummaryMinCadence;
        public double SummaryMinCadence
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMinCadence;
            }
            private set
            {
                this._SummaryMinCadence = value;
            }
        }

        double _SummaryMaxTemperature;
        public double SummaryMaxTemperature
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxTemperature;
            }
            private set
            {
                this._SummaryMaxTemperature = value;
            }
        }

        double _SummaryMinTemperature;
        public double SummaryMinTemperature
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMinTemperature;
            }
            private set
            {
                this._SummaryMinTemperature = value;
            }
        }

        double _SummaryMaxAltitude;
        public double SummaryMaxAltitude
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxAltitude;
            }
            private set
            {
                this._SummaryMaxAltitude = value;
            }
        }

        double _SummaryMinAltitude;
        public double SummaryMinAltitude
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMinAltitude;
            }
            private set
            {
                this._SummaryMinAltitude = value;
            }
        }

        double _SummaryMaxMomentSlope;
        public double SummaryMaxMomentSlope
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxMomentSlope;
            }
            private set
            {
                this._SummaryMaxMomentSlope = value;
            }
        }

        double _SummaryMinMomentSlope;
        public double SummaryMinMomentSlope
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMinMomentSlope;
            }
            private set
            {
                this._SummaryMinMomentSlope = value;
            }
        }

        public double SummaryMaxPeriodSlope
        {
            get
            {
                if (this.SlopeData.Count > 0)
                {
                    double slope =
                        this.SlopeData.Max((e) => { return !double.IsNaN(e.Slope) ? e.Slope : double.MinValue; });

                    slope = slope > double.MinValue ? slope : 0;

                    return slope;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SummaryMinPeriodSlope
        {
            get
            {
                if (this.SlopeData.Count > 0)
                {
                    double slope =
                        this.SlopeData.Min((e) => { return !double.IsNaN(e.Slope) ? e.Slope : double.MaxValue; });

                    slope = slope < double.MaxValue ? slope : 0;

                    return slope;
                }
                else
                {
                    return 0;
                }
            }
        }

        double _SummaryMaxPower;
        public double SummaryMaxPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxPower;
            }
            private set
            {
                this._SummaryMaxPower = value;
            }
        }

        double _SummaryMaxLeftLegPower;
        public double SummaryMaxLeftLegPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxLeftLegPower;
            }
            private set
            {
                this._SummaryMaxLeftLegPower = value;
            }
        }

        double _SummaryMaxRightLegPower;
        public double SummaryMaxRightLegPower
        {
            get
            {
                this.UpdateSummaryData();
                return this._SummaryMaxRightLegPower;
            }
            private set
            {
                this._SummaryMaxRightLegPower = value;
            }
        }

        private double _SummaryMax20MinutesPower;

        public double SummaryMax20MinutesPower
        {
            get
            {
                if (_SummaryMax20MinutesPowerStartTime.Subtract(new DateTime()).TotalMilliseconds <= 10000)
                {
                    this.GetMax20MinituesData();
                }

                return _SummaryMax20MinutesPower;
            }
            set
            {
                _SummaryMax20MinutesPower = value;
            }
        }

        private DateTime _SummaryMax20MinutesPowerStartTime = new DateTime();

        public DateTime SummaryMax20MinutesPowerStartTime
        {
            get
            {
                if (_SummaryMax20MinutesPowerStartTime.Subtract(new DateTime()).TotalMilliseconds <= 10000)
                {
                    this.GetMax20MinituesData();
                }

                return _SummaryMax20MinutesPowerStartTime;
            }
        }

        public DateTime SummaryMax20MinutesPowerEndTime { get; set; }

        private double _SummaryMax20MinutesHeartRate;

        public double SummaryMax20MinutesHeartRate
        {
            get
            {
                if (_SummaryMax20MinutesHeartRateStartTime.Subtract(new DateTime()).TotalMilliseconds <= 10000)
                {
                    this.GetMax20MinituesData();
                }

                return _SummaryMax20MinutesHeartRate;
            }
        }

        private DateTime _SummaryMax20MinutesHeartRateStartTime = new DateTime();

        public DateTime SummaryMax20MinutesHeartRateStartTime
        {
            get
            {
                if (_SummaryMax20MinutesHeartRateStartTime.Subtract(new DateTime()).TotalMilliseconds <= 10000)
                {
                    this.GetMax20MinituesData();
                }

                return _SummaryMax20MinutesHeartRateStartTime;
            }
        }

        public DateTime SummaryMax20MinutesHeartRateEndTime { get; set; }

        private double _SummaryNormalizedPower = double.NaN;
        public double SummaryNormalizedPower
        {
            get
            {
                int NPSplitTotalSeconds = 30;

                if (double.IsNaN(_SummaryNormalizedPower))
                {
                    if (this.DataPoints.Count > 1 && this._SummaryMax20MinutesPower > 0)
                    {
                        double timeRange = this.DataPoints.Last().Time.Subtract(this.DataPoints.First().Time).TotalSeconds > NPSplitTotalSeconds ? NPSplitTotalSeconds : 1;

                        DateTime startTime = this.DataPoints.First().Time;

                        List<object[]> powerAvg10s = new List<object[]>();

                        #region get 30s avg power
                        for (int i = 0; i < this.DataPoints.Count; i++)
                        {
                            var currentPoint = this.DataPoints[i];

                            double current = currentPoint.Power.TotalPower * currentPoint.ElapsedTime.TotalSeconds;
                            double totalElapsed = currentPoint.ElapsedTime.TotalSeconds;

                            long currentTime = i == 0 ? new TimeSpan(0, 0, 1).Ticks : currentPoint.ElapsedTime.Ticks;
                            int count = 1;

                            for (int j = i + 1; j < this.DataPoints.Count; j++)
                            {
                                var jPoint = this.DataPoints[j];

                                TimeSpan ts = jPoint.Time.Subtract(currentPoint.Time);

                                current += jPoint.Power.TotalPower * jPoint.ElapsedTime.TotalSeconds;
                                totalElapsed += jPoint.ElapsedTime.TotalSeconds;
                                currentTime += jPoint.ElapsedTime.Ticks;
                                count++;

                                if (j == this.DataPoints.Count - 1)
                                {
                                    j++;
                                }

                                if (ts.TotalSeconds >= timeRange || j == this.DataPoints.Count)
                                {
                                    object[] p = new object[]
                                    {
                                        (totalElapsed > 0 && current > 0) ? current / totalElapsed : 0,
                                        new TimeSpan(currentTime)
                                    };

                                    //if ((double)p[0] > 0 || powerAvg10s.Count > 0)
                                    {
                                        powerAvg10s.Add(p);
                                    }

                                    i = j;
                                    break;
                                }
                            }
                        }
                        #endregion

                        #region collect power data - time
                        var powerList = from a in powerAvg10s
                                        select (int)a[0];

                        var distinctPower = powerList.Distinct();

                        Dictionary<int, TimeSpan> powerTimeList = new Dictionary<int, TimeSpan>();

                        foreach (object[] powerData in powerAvg10s)
                        {
                            int power = Convert.ToInt32(Math.Round((double)powerData[0]));
                            TimeSpan powerTime = (TimeSpan)powerData[1];
                            if (powerTimeList.ContainsKey(power))
                            {
                                powerTimeList[power].Add(powerTime);
                            }
                            else
                            {
                                powerTimeList.Add(power, powerTime);
                            }
                        }
                        #endregion

                        #region calculate NP
                        double np = 0;
                        foreach (KeyValuePair<int, TimeSpan> powerD in powerTimeList)
                        { // np = 165^4 * 35 + 273^4 * 28 + 192^4 * 15...
                            np += Math.Pow(powerD.Key, 4) * powerD.Value.TotalSeconds;
                        }

                        np = np / powerTimeList.Sum((e) => { return e.Value.TotalSeconds; });

                        np = Math.Pow(np, 0.25);

                        #endregion

                        this._SummaryNormalizedPower = np;
                        return np;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return this._SummaryNormalizedPower;
                }
            }
        }

        /// <summary>
        /// if FTP has been set, use set FTP. otherwise, use 20min power * 0.95 as FTP
        /// </summary>
        public double SummaryIntensityFactor
        {
            get
            {
                if (this._SummaryMax20MinutesPower > 0)
                {
                    double ftp = this.FTP;

                    if (ftp <= 0)
                    {
                        ftp = this._SummaryMax20MinutesPower * 0.95;
                    }

                    return this.SummaryNormalizedPower / ftp;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double SummaryTrainingStressScore
        {
            get
            {
                double ifValue = this.SummaryIntensityFactor;
                return ifValue * ifValue * this.SummaryTotalMoveTime.TotalHours * 100;
            }
        }


        public double SummaryLeftPowerPhaseStart
        {
            get { this.UpdateSummaryData(); return _SummaryLeftPowerPhaseStart; }
        }

        public double SummaryLeftPowerPhaseEnd
        {
            get { this.UpdateSummaryData(); return _SummaryLeftPowerPhaseEnd; }
        }


        public double SummaryLeftPowerPhasePeakStart
        {
            get { this.UpdateSummaryData(); return _SummaryLeftPowerPhasePeakStart; }
        }

        public double SummaryLeftPowerPhasePeakEnd
        {
            get { this.UpdateSummaryData(); return _SummaryLeftPowerPhasePeakEnd; }
        }


        public double SummaryLeftPlatformCenterOffset
        {
            get { this.UpdateSummaryData(); return _SummaryLeftPlatformCenterOffset; }
        }

        public double SummaryRightPowerPhaseStart
        {
            get { this.UpdateSummaryData(); return _SummaryRightPowerPhaseStart; }
        }

        public double SummaryRightPowerPhaseEnd
        {
            get { this.UpdateSummaryData(); return _SummaryRightPowerPhaseEnd; }
        }

        public double SummaryRightPowerPhasePeakStart
        {
            get { this.UpdateSummaryData(); return _SummaryRightPowerPhasePeakStart; }
        }

        public double SummaryRightPowerPhasePeakEnd
        {
            get { this.UpdateSummaryData(); return _SummaryRightPowerPhasePeakEnd; }
        }

        public double SummaryRightPlatformCenterOffset
        {
            get { this.UpdateSummaryData(); return _SummaryRightPlatformCenterOffset; }
        }

        public TimeSpan SummaryTimeStanding
        {
            get
            {
                ////TimeSpan ts = new TimeSpan();

                ////if (this._Laps != null)
                ////{
                ////    foreach (var lap in this._Laps)
                ////    {
                ////        ts = ts.Add(lap.TimeStanding);
                ////    }
                ////}

                ////return ts;
                this.UpdateSummaryData(); return _SummaryTotalStandTime;
            }
        }

        /// <summary>
        /// 80 means 80%
        /// </summary>
        public double SummaryStandingPercentage
        {
            get
            {
                if (this.SummaryTotalMoveTime.TotalSeconds > 0)
                    return this.SummaryTimeStanding.TotalSeconds / this._SummaryTotalMoveTime.TotalSeconds * 100;

                return 0;
            }
        }

        public int SummaryStandCount
        {
            get
            {
                ////int ts = 0;

                ////if (this._Laps != null)
                ////{
                ////    foreach (var lap in this._Laps)
                ////    {
                ////        ts += lap.StandCount;
                ////    }
                ////}

                ////return ts;
                this.UpdateSummaryData(); return _SummaryTotalStandCount;
            }
        }
        #endregion

        #region functions
        /// <summary>
        /// Get slope datas. For example, return 0m, 100m, 150m, 200m it will use those points as slope mark.
        /// </summary>
        /// <param name="slopeSplitMeter"></param>
        public void GetSlopeData(int slopeSplitMeter)
        {
            this.GetSlopeData(this.DataPoints, slopeSplitMeter);
        }

        public void GetExtraData(int slopeSplitMeter)
        {
            this.GetSlopeData(slopeSplitMeter);

            this.GetRangeData(this.LTHR, this.FTP);
        }

        /// <summary>
        /// GetSlopeData. For example, return 0m, 100m, 150m, 200m it will use those points as slope mark.
        /// </summary>
        /// <param name="dataPointSource"></param>
        /// <param name="slopeSplitMeter"></param>
        /// <returns></returns>
        public void GetSlopeData(List<CyclingDataPoint> dataPointSource, int slopeSplitMeter)
        {
            if (2 <= dataPointSource.Count)
            {
                this.SlopeData.Clear();

                List<CyclingDataPoint> tempList = new List<CyclingDataPoint>();
                CyclingDataPoint lastPoint = dataPointSource[0];
                tempList.Add(lastPoint);

                double highestAlti = this.DataPoints.Max<CyclingDataPoint>((e) => { return e.Position.AltitudeMeters; });
                double lowestAlti = this.DataPoints.Min<CyclingDataPoint>((e) => { return e.Position.AltitudeMeters; });
                double lastSlope = 0;
                bool isRecalculateSlope = true;

                for (int i = 1; i < dataPointSource.Count; i++)
                {
                    CyclingDataPoint currentIPoint = dataPointSource[i];

                    double lastHMeter = Math.Sqrt(Math.Pow(currentIPoint.DistanceMeter - lastPoint.DistanceMeter, 2) - Math.Pow(currentIPoint.Position.AltitudeMeters - lastPoint.Position.AltitudeMeters, 2));

                    if (slopeSplitMeter > 0 && i <= 1)
                    {
                        lastSlope = (currentIPoint.Position.AltitudeMeters - lastPoint.Position.AltitudeMeters)
                                    / lastHMeter * 100;

                        tempList.Add(currentIPoint);
                        continue;
                    }

                    if (i == dataPointSource.Count - 1
                        || lastHMeter >= slopeSplitMeter)
                    { // current point - last start point >= set value, such as 200 - 100 >= 100, then add slope data

                        tempList.Add(currentIPoint);

                        SlopeType slope = new SlopeType(tempList.ToArray());
                        this.SlopeData.Add(slope);

                        tempList.Clear();
                        lastPoint = currentIPoint;

                        lastSlope = slope.Slope;
                        isRecalculateSlope = false;

                        tempList.Add(currentIPoint);

                        continue;
                    }


                    if (currentIPoint.Position.DeltaDistanceMeters <= 1 && Math.Abs(currentIPoint.Position.DeltaAltitudeMeters) >= 1)
                    { // SuddenChangePoint
                        SlopeType slope = new SlopeType(tempList.ToArray());
                        this.SlopeData.Add(slope);

                        tempList.Clear();
                        lastPoint = currentIPoint;

                        lastSlope = slope.Slope;
                        isRecalculateSlope = false;

                        tempList.Add(currentIPoint);

                        continue;
                    }

                    //if (this._SlopeSplitSeconds <= 0 && this._SlopeSplitMeter > 0)
                    { // slope second is 0 and slope meter > 0 , and current point is max or min point. Add current point into last list.
                        if (currentIPoint.Position.AltitudeMeters >= highestAlti || currentIPoint.Position.AltitudeMeters <= lowestAlti)
                        {
                            bool recordMaxOrMin = true;

                            if (0 < this.SlopeData.Count)
                            {
                                SlopeType slope = this.SlopeData.Last();

                                if (0 < slope.DataPoints.Count && slope.DataPoints.Last().Position.AltitudeMeters == currentIPoint.Position.AltitudeMeters)
                                {
                                    recordMaxOrMin = false;
                                }
                            }

                            if (recordMaxOrMin)
                            {
                                tempList.Add(currentIPoint);

                                SlopeType slope = new SlopeType(tempList.ToArray());
                                this.SlopeData.Add(slope);

                                tempList.Clear();
                                lastPoint = currentIPoint;

                                lastSlope = slope.Slope;
                                isRecalculateSlope = false;

                                tempList.Add(currentIPoint);

                                continue;
                            }
                        }
                    }


                    #region deal with next part
                    // Get last slope
                    ////lastSlope = (currentIPoint.Position.AltitudeMeters - lastPoint.Position.AltitudeMeters)
                    ////    / (currentIPoint.DistanceMeter - lastPoint.DistanceMeter) * 100;
                    double nextSlope = lastSlope;

                    // Slope spliter analyzer. Min analyzer will be 10s distance, or input value.
                    // 10s distance = speed (km/h) * 1000 (m/h) / 3600 (m/s) * 10
                    double slopeSplitAnalyzer = Math.Min(slopeSplitMeter, (currentIPoint.Speed > 0 ? currentIPoint.Speed : int.MaxValue) / 3.6 * this.ParamSlopeSplitMinSeconds);

                    ////if (currentIPoint.DistanceMeter - lastPoint.DistanceMeter < slopeSplitAnalyzer && 0 < this.SlopeData.Count)
                    ////{   // Only if current point to last point is longer than calculate analyzer data, to judge the trend
                    ////    lastSlope = this.SlopeData[this.SlopeData.Count - 1].Slope;
                    ////}

                    if (isRecalculateSlope)
                    {   // if current point to last point is shorter than calculate analyzer data, and this is first slope data, calculate last slope
                        lastSlope = (currentIPoint.Position.AltitudeMeters - lastPoint.Position.AltitudeMeters)
                                     / lastHMeter * 100;
                    }


                    List<CyclingDataPoint> tempListJ = new List<CyclingDataPoint>();
                    tempListJ.Add(currentIPoint);

                    for (int j = i + 1; j < dataPointSource.Count - 1; j++)
                    {
                        CyclingDataPoint currentJPoint = dataPointSource[j];

                        if (currentJPoint.Position.DeltaDistanceMeters <= 1 && Math.Abs(currentJPoint.Position.DeltaAltitudeMeters) >= 1)
                        { // Skip 0 speed point (Very large slope data)
                            break;
                        }

                        double distDiff = currentJPoint.DistanceMeter - currentIPoint.DistanceMeter;
                        double lastJSubIHMeter = Math.Sqrt(Math.Pow(distDiff, 2) - Math.Pow(currentJPoint.Position.AltitudeMeters - currentIPoint.Position.AltitudeMeters, 2));//
                        double slopeSplitAnalyzerJ = Math.Min(slopeSplitMeter, (currentJPoint.Speed > 0 ? currentJPoint.Speed : int.MaxValue) / 3.6 * this.ParamSlopeSplitMinSeconds);

                        if (distDiff >= slopeSplitAnalyzerJ)
                        {
                            if (lastHMeter >= slopeSplitAnalyzer)
                            {  // Only if last save distance more than slope split analyzer, just save the point into list
                                nextSlope = (currentJPoint.Position.AltitudeMeters - currentIPoint.Position.AltitudeMeters)
                                / lastJSubIHMeter * 100;

                                if ((nextSlope * lastSlope > 0 && (nextSlope - lastSlope >= 1 || lastSlope - nextSlope >= 2))
                                    || ((nextSlope * lastSlope) < 0 && Math.Abs(nextSlope - lastSlope) >= 1))
                                {   // next slope and last slope both > 0, and next slope larger than next slope more than 1,
                                    // Or next and last slope is different (one > 0 and another < 0) and different more than 1
                                    // so record I point into slope list
                                    tempList.Add(currentIPoint);

                                    SlopeType slope = new SlopeType(tempList.ToArray());
                                    this.SlopeData.Add(slope);

                                    tempList.Clear();
                                    lastPoint = currentIPoint;

                                    lastSlope = nextSlope;

                                    isRecalculateSlope = false;
                                }
                            }

                            break;
                        }
                        else
                        {
                            #region JPoint is highest or lowest point
                            if (currentJPoint.Position.AltitudeMeters >= highestAlti || currentJPoint.Position.AltitudeMeters <= lowestAlti)
                            { // Current J point is highest or Lowest point, and the distance is not as long as Analyzer meter.
                                // direct add highest or lowest point into slope list.
                                bool recordMaxOrMin = true;

                                if (0 < this.SlopeData.Count)
                                {
                                    SlopeType slope = this.SlopeData.Last();

                                    if (0 < slope.DataPoints.Count && slope.DataPoints.Last().Position.AltitudeMeters == currentJPoint.Position.AltitudeMeters)
                                    {
                                        recordMaxOrMin = false;
                                    }
                                }

                                if (recordMaxOrMin)
                                {
                                    #region add J tops
                                    double tempSlope = (currentJPoint.Position.AltitudeMeters - currentIPoint.Position.AltitudeMeters)
                                        / lastJSubIHMeter * 100;

                                    if (Math.Abs(tempSlope - lastSlope) >= 2)
                                    { // Save I point as new point and then save J point as new point
                                        tempListJ.Add(currentJPoint);

                                        tempList.Add(currentIPoint);

                                        SlopeType slope = new SlopeType(tempList.ToArray());
                                        this.SlopeData.Add(slope);

                                        tempList.Clear();


                                        SlopeType slopeJ = new SlopeType(tempListJ.ToArray());
                                        this.SlopeData.Add(slopeJ);

                                        tempListJ.Clear();

                                        lastPoint = currentJPoint;

                                        i = j;
                                        currentIPoint = currentJPoint;

                                        // JPoint is highest or lowest point, means next slope will always different with last slope.
                                        // So re-calculate slope by next point.
                                        isRecalculateSlope = true;

                                        break;
                                    }
                                    else
                                    {  // Appand J point into last record
                                        if (lastPoint.Time == currentIPoint.Time)
                                        { // last point as the same as IPoint
                                            if (this.SlopeData.Count > 0)
                                            { // will be always > 0. Because i start at 1, and if no record, lastPoint will be at 0
                                                tempListJ.Add(currentJPoint);

                                                this.SlopeData[this.SlopeData.Count - 1].DataPoints.AddRange(tempListJ);

                                                tempList.Clear();
                                                tempListJ.Clear();

                                                i = j;
                                                currentIPoint = currentJPoint;

                                                lastPoint = currentJPoint;

                                                // JPoint is highest or lowest point, means next slope will always different with last slope.
                                                // So re-calculate slope by next point.
                                                isRecalculateSlope = true;

                                                break;
                                            }
                                        }
                                        else
                                        { // last point is not current IPoint, save last point to JPoint as new slope
                                            tempListJ.Add(currentJPoint);

                                            tempList.AddRange(tempListJ);

                                            SlopeType slope = new SlopeType(tempList.ToArray());
                                            this.SlopeData.Add(slope);

                                            tempList.Clear();
                                            tempListJ.Clear();

                                            lastPoint = currentJPoint;

                                            i = j;
                                            currentIPoint = currentJPoint;

                                            // JPoint is highest or lowest point, means next slope will always different with last slope.
                                            // So re-calculate slope by next point.
                                            isRecalculateSlope = true;

                                            break;
                                        }
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }

                        tempListJ.Add(currentJPoint);
                    }


                    #endregion

                    tempList.Add(currentIPoint);

                }
            }
        }

        public void GetMax20MinituesData()
        {
            if (this.DataPoints.Count > 1)
            {
                this._SummaryMax20MinutesHeartRateStartTime = this._SummaryMax20MinutesPowerStartTime = this.DataPoints.First().Time;
                this.SummaryMax20MinutesHeartRateEndTime = this.SummaryMax20MinutesPowerEndTime = this.DataPoints.Last().Time;

                int si, ei;
                var maxP = this.GetMaxNPowerData(1200, out si, out ei);

                if (maxP != null && si >= 0 && ei >= 0)
                {
                    this._SummaryMax20MinutesPowerStartTime = this._DataPoints[si].Time;
                    this.SummaryMax20MinutesPowerEndTime = this._DataPoints[ei].Time;
                    this._SummaryMax20MinutesPower = maxP.SummaryAvgPower;
                }

                maxP = this.GetMaxNPowerData(1200, out si, out ei, true);

                if (maxP != null && si >= 0 && ei >= 0)
                {
                    this._SummaryMax20MinutesHeartRateStartTime = this._DataPoints[si].Time;
                    this.SummaryMax20MinutesHeartRateEndTime = this._DataPoints[ei].Time;
                    this._SummaryMax20MinutesHeartRate = maxP.SummaryAvgHeartRate;
                }
            }
        }

        /////// <summary>
        /////// Get max period power data
        /////// </summary>
        /////// <param name="totalSeconds">calculate total seconds</param>
        /////// <param name="startTime"></param>
        /////// <param name="endTime"></param>
        /////// <returns>Is contains enough data (For example, current record contains 100s data but totalSeconds set to 120s, return false)</returns>
        ////public bool GetMaxPeriodPowerData(double totalSeconds, out double maxPeriodPower, out DateTime startTime, out DateTime endTime)
        ////{
        ////    startTime = new DateTime();
        ////    endTime = new DateTime();
        ////    maxPeriodPower = 0;

        ////    bool result = false;

        ////    if (this.DataPoints.Count > 1)
        ////    {
        ////        double max = 0;

        ////        maxPeriodPower = max = -1;//this.SummaryAvgPower;
        ////        startTime = this.DataPoints.First().Time;
        ////        endTime = this._DataPoints.Last().Time;

        ////        for (int i = 0; i < this.DataPoints.Count; i++)
        ////        {
        ////            var currentPoint = this.DataPoints[i];
        ////            double current = currentPoint.Power.TotalPower * currentPoint.ElapsedTime.TotalSeconds;
        ////            double totalElapsed = currentPoint.ElapsedTime.TotalSeconds;

        ////            for (int j = i + 1; j < this.DataPoints.Count; j++)
        ////            {
        ////                var jPoint = this.DataPoints[j];

        ////                if (jPoint.Time.Subtract(currentPoint.Time).TotalSeconds >= totalSeconds)
        ////                {
        ////                    current = current / totalElapsed;

        ////                    max = Math.Max(current, max);

        ////                    if (current == max)
        ////                    {
        ////                        startTime = currentPoint.Time;
        ////                        endTime = jPoint.Time;
        ////                        maxPeriodPower = max;

        ////                        result = true;
        ////                    }

        ////                    break;
        ////                }

        ////                current += jPoint.Power.TotalPower * jPoint.ElapsedTime.TotalSeconds;
        ////                totalElapsed += currentPoint.ElapsedTime.TotalSeconds;
        ////            }
        ////        }

        ////        if (0 >= max)
        ////        {
        ////            maxPeriodPower = this.SummaryAvgPower;
        ////        }
        ////    }

        ////    return result;
        ////}

        /// <summary>
        /// percentage will be: 80 -> 80% of currentEndTime - currentStartTime
        /// </summary>
        /// <param name="startPercentage"></param>
        /// <param name="endPercentage"></param>
        /// <returns></returns>
        public CyclingData GetSubData(double startPercentage, double endPercentage, int slopeSplitMinSeconds = int.MinValue, int slopeSplitMeter = int.MinValue)
        {
            CyclingData output = this;

            if (this.DataPoints.Count > 1)
            {
                DateTime startTime = this.DataPoints.First().Time;
                DateTime endTime = this.DataPoints.Last().Time;
                long tick = endTime.Subtract(startTime).Ticks;

                DateTime newStartTime = startTime.AddTicks((long)((double)tick / 100 * startPercentage));
                DateTime newEndTime = startTime.AddTicks((long)((double)tick / 100 * endPercentage));

                output = this.GetSubData(newStartTime, newEndTime, slopeSplitMinSeconds, slopeSplitMeter);
            }

            return output;
        }

        /// <summary>
        /// Time can be 2014/1/30 17:55:25, or 17:55:25. Or, can be percentage.percentage will be: 80 -> 80% of currentEndTime - currentStartTime
        /// </summary>
        /// <param name="startTimeStr"></param>
        /// <param name="endTimeStr"></param>
        /// <returns></returns>
        public CyclingData GetSubData(string startStr, string endStr, int slopeSplitMinSeconds = int.MinValue, int slopeSplitMeter = int.MinValue)
        {
            CyclingData output = this;

            if (this.DataPoints.Count > 1)
            {
                DateTime startTime = this.DataPoints.First().Time;
                DateTime endTime = this.DataPoints.Last().Time;

                DateTime enterStartTime;
                DateTime enterEndTime;

                if ((DateTime.TryParse(startStr, out enterStartTime) && DateTime.TryParse(endStr, out enterEndTime))
                    || (
                        DateTime.TryParseExact(
                            startStr,
                            new string[] { "yyyy/MM/dd HH:mm:ss", "yyyy/M/d H:m:s", "HH:mm:ss", "H:m:s" },
                            CultureInfo.GetCultureInfo("en-US"),
                            DateTimeStyles.None,
                            out enterStartTime)
                        && DateTime.TryParseExact(
                            endStr,
                            new string[] { "yyyy/MM/dd HH:mm:ss", "yyyy/M/d H:m:s", "HH:mm:ss", "H:m:s" },
                            CultureInfo.GetCultureInfo("en-US"),
                            DateTimeStyles.None,
                            out enterEndTime)
                    ))
                {
                    DateTime temp;
                    if (DateTime.TryParseExact(
                            startStr,
                            new string[] { "HH:mm:ss", "H:m:s" },
                            CultureInfo.GetCultureInfo("en-US"),
                            DateTimeStyles.None,
                            out temp))
                    {
                        enterStartTime = new DateTime(startTime.Year, startTime.Month, startTime.Day,
                            enterStartTime.Hour, enterStartTime.Minute, enterStartTime.Second, enterStartTime.Millisecond);
                    }

                    if (DateTime.TryParseExact(
                            endStr,
                            new string[] { "HH:mm:ss", "H:m:s" },
                            CultureInfo.GetCultureInfo("en-US"),
                            DateTimeStyles.None,
                            out temp))
                    {
                        enterEndTime = new DateTime(endTime.Year, endTime.Month, endTime.Day,
                            enterEndTime.Hour, enterEndTime.Minute, enterEndTime.Second, enterEndTime.Millisecond);
                    }

                    output = this.GetSubData(enterStartTime, enterEndTime, slopeSplitMinSeconds, slopeSplitMeter);
                }
                else
                {
                    double startPer;
                    double endPer;

                    if (double.TryParse(startStr, out startPer) && double.TryParse(endStr, out endPer))
                    {
                        output = this.GetSubData(startPer, endPer, slopeSplitMinSeconds, slopeSplitMeter);
                    }
                }
            }

            return output;
        }

        public CyclingData GetSubData(DateTime startTime, DateTime endTime, int slopeSplitMinSeconds = int.MinValue, int slopeSplitMeter = int.MinValue)
        {
            CyclingData output = this;

            if (endTime.Subtract(startTime).Ticks < 0)
            {
                DateTime temp = startTime;
                startTime = endTime;
                endTime = temp;
            }

            if (endTime.Subtract(startTime).TotalSeconds > 2)
            {
                //output = new CyclingData();
                List<CyclingDataPoint> lst = new List<CyclingDataPoint>();

                foreach (CyclingDataPoint point in this.DataPoints)
                {
                    if (point.Time >= startTime && point.Time <= endTime)
                    {
                        lst.Add(point);
                    }

                    if (point.Time > endTime)
                    {
                        break;
                    }
                }

                //output.ParamSlopeSplitMinSeconds = this.ParamSlopeSplitMinSeconds;
                //output.ParamSlopeSplitMeter = this.ParamSlopeSplitMeter;

                output = new CyclingData(lst.ToArray(), this);
            }

            if (slopeSplitMeter >= 0)
            {
                output.ParamSlopeSplitMeter = slopeSplitMeter;
            }

            if (slopeSplitMinSeconds >= 0)
            {
                output.ParamSlopeSplitMinSeconds = slopeSplitMinSeconds;
            }


            return output;
        }

        /// <summary>
        /// If seconds <= 1, return maxPoint.
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="isGetHR"></param>
        /// <returns></returns>
        public CyclingData GetMaxNPowerData(double seconds, out int startIndex, out int endIndex, bool isGetHR = false)
        {
            startIndex = -1;
            endIndex = -1;

            if (this._DataPoints.Count <= 0)
            {
                return null;
            }

            if (seconds <= 1)
            {
                var maxP = this._DataPoints.Where(p => p.Power.TotalPower >= this.SummaryMaxPower).First();
                startIndex = this._DataPoints.IndexOf(maxP);
                endIndex = startIndex;
                return new CyclingData(new CyclingDataPoint[] { maxP });
            }

            double currentMax = -1;

            int currentStartIndex = 0;
            double tempTotalPower = 0;
            double tempTotalSeconds = 0;

            for (int i = 0; i < this._DataPoints.Count; i++)
            {
                var currentP = this._DataPoints[i];
                var currentES = currentP.ElapsedTime.TotalSeconds;
                tempTotalPower += (isGetHR ? currentP.HeartRateBpm : currentP.Power.TotalPower) * currentES;
                tempTotalSeconds += currentES;

                if (tempTotalSeconds >= seconds)
                {
                    double avgPower = tempTotalSeconds > 0 ? tempTotalPower / tempTotalSeconds : 0;

                    if (currentMax < avgPower)
                    {
                        currentMax = avgPower;
                        startIndex = currentStartIndex;
                        endIndex = i;
                    }

                    do
                    {
                        var startP = this._DataPoints[currentStartIndex];
                        var startES = startP.ElapsedTime.TotalSeconds;
                        tempTotalPower -= (isGetHR ? startP.HeartRateBpm : startP.Power.TotalPower) * startES;
                        tempTotalSeconds -= startES;

                        if (tempTotalSeconds >= seconds)
                        {
                            avgPower = tempTotalSeconds > 0 ? tempTotalPower / tempTotalSeconds : 0;

                            if (currentMax < avgPower)
                            {
                                currentMax = avgPower;
                                startIndex = currentStartIndex;
                                endIndex = i;
                            }
                        }

                        currentStartIndex++;
                    }
                    while (currentStartIndex < i && tempTotalSeconds >= seconds);
                }
            }

            if (startIndex >= 0 && endIndex >= 0)
            {
                var points = this._DataPoints.GetRange(startIndex, endIndex - startIndex + 1).ToArray();

                return new CyclingData(points, this);
            }
            else
            {
                startIndex = 0;
                endIndex = this._DataPoints.Count - 1;

                return this;
            }

            return null;
        }

        public CyclingDataPoint GetMaxNPowerDataPoint(CyclingDataPoint[] points)
        {
            if (points == null || points.Length <= 0)
            {
                return null;
            }

            CyclingData tempC = new CyclingData(points.ToArray());

            var p = new CyclingDataPoint();
            var firstP = points.First();

            p.Time = firstP.Time;
            p.DistanceMeter = firstP.DistanceMeter;

            p.Position.LatitudeDegrees = firstP.Position.LatitudeDegrees;
            p.Position.LongitudeDegrees = firstP.Position.LongitudeDegrees;
            p.Position.AltitudeMeters = firstP.Position.AltitudeMeters;

            p.Cadence = tempC.SummaryAvgCadence;
            p.ElapsedTime = new TimeSpan(tempC.DataPoints.Sum(pp => pp.ElapsedTime.Ticks));//tempC.SummaryTotalMoveTime;
            p.HeartRateBpm = tempC.SummaryAvgHeartRate;
            p.Position.DeltaAltitudeMeters = tempC.SummaryTotalAltitudeGain - tempC.SummaryTotalAltitudeLost;
            p.Position.DeltaDistanceMeters = tempC.DataPoints.Sum(pp => pp.Position.DeltaDistanceMeters);
            p.Power.AccumulatedPower = tempC.SummaryAvgAccumulatedPower;
            p.Power.LeftLegPower = tempC.SummaryAvgLeftLegPower;
            p.Power.LeftPedalSmoothness = tempC.SummaryAvgLeftPedalSmoothness;
            p.Power.LeftTorqueEffectiveness = tempC.SummaryAvgLeftTorqueEffectiveness;
            p.Power.PedalSmoothness = tempC.SummaryAvgPedalSmoothness;
            p.Power.RightLegPower = tempC.SummaryAvgRightLegPower;
            p.Power.RightPedalSmoothness = tempC.SummaryAvgRightPedalSmoothness;
            p.Power.RightTorqueEffectiveness = tempC.SummaryAvgRightTorqueEffectiveness;
            p.Power.TorqueEffectiveness = tempC.SummaryAvgTorqueEffectiveness;
            p.Power.TotalPower = tempC.SummaryAvgPower;

            p.Power.LeftPowerPhaseStart = tempC.SummaryLeftPowerPhaseStart;
            p.Power.LeftPowerPhaseEnd = tempC.SummaryLeftPowerPhaseEnd;
            p.Power.LeftPowerPhasePeakStart = tempC.SummaryLeftPowerPhasePeakStart;
            p.Power.LeftPowerPhasePeakEnd = tempC.SummaryLeftPowerPhasePeakEnd;
            p.Power.LeftPlatformCenterOffset = tempC.SummaryLeftPlatformCenterOffset;

            p.Power.RightPowerPhaseStart = tempC.SummaryRightPowerPhaseStart;
            p.Power.RightPowerPhaseEnd = tempC.SummaryRightPowerPhaseEnd;
            p.Power.RightPowerPhasePeakStart = tempC.SummaryRightPowerPhasePeakStart;
            p.Power.RightPowerPhasePeakEnd = tempC.SummaryRightPowerPhasePeakEnd;
            p.Power.RightPlatformCenterOffset = tempC.SummaryRightPlatformCenterOffset;

            p.Speed = points.Length > 1 ? tempC.SummaryAvgSpeed : firstP.Speed;
            p.Temperature = tempC.SummaryAvgTemperature;

            return p;
        }

        public CyclingData GetMaxNPowerBasedSplitData(int durationSeconds)
        {
            int startIndex;
            int endIndex;

            if (this._DataPoints.Count > 0)
            {
                var maxPower = this.GetMaxNPowerData(durationSeconds, out startIndex, out endIndex);

                if (startIndex < 0 || endIndex < 0)
                {
                    maxPower = null;
                    startIndex = endIndex = this._DataPoints.Count - 1;
                }

                var newPoints = new List<CyclingDataPoint>();
                var tempPoints = new List<CyclingDataPoint>();

                #region Add before part
                if (startIndex > 0)
                {
                    double totalEllapsedMilliSeconds = 0;

                    for (int i = startIndex - 1; i >= 0; i--)
                    {
                        tempPoints.Insert(0, this._DataPoints[i]);
                        totalEllapsedMilliSeconds += this._DataPoints[i].ElapsedTime.TotalMilliseconds;

                        if (totalEllapsedMilliSeconds / 1000.0 >= durationSeconds)
                        {
                            this.AddPointsToResult(ref newPoints, ref tempPoints, true);

                            totalEllapsedMilliSeconds = 0;
                        }
                    }

                    if (tempPoints.Count > 0)
                    {
                        this.AddPointsToResult(ref newPoints, ref tempPoints, true);
                    }
                }
                #endregion

                #region Add max part
                if (maxPower != null && maxPower.DataPoints.Count > 0)
                {
                    var tempP = new List<CyclingDataPoint>(maxPower.DataPoints);

                    this.AddPointsToResult(ref newPoints, ref tempP);
                }
                #endregion

                #region Add next part
                if (endIndex < this._DataPoints.Count - 1)
                {
                    double totalEllapsedMilliSeconds = 0;

                    for (int i = endIndex + 1; i < this._DataPoints.Count; i++)
                    {
                        tempPoints.Add(this._DataPoints[i]);
                        totalEllapsedMilliSeconds += this._DataPoints[i].ElapsedTime.TotalMilliseconds;

                        if (totalEllapsedMilliSeconds / 1000.0 >= durationSeconds)
                        {
                            this.AddPointsToResult(ref newPoints, ref tempPoints);

                            totalEllapsedMilliSeconds = 0;
                        }
                    }

                    if (tempPoints.Count > 0)
                    {
                        this.AddPointsToResult(ref newPoints, ref tempPoints);
                    }
                }
                #endregion

                if (newPoints.Count > 0)
                {
                    #region add blank first P to make trendLine flat
                    var firstP = this._DataPoints.First();
                    var newPS = this.GetMaxNPowerDataPoint(new CyclingDataPoint[] { newPoints.First() });

                    //newPoints.First().Time = newPoints.First().Time.Add(firstP.ElapsedTime);
                    //newPoints.First().DistanceMeter = firstP.Position.DeltaDistanceMeters;

                    newPS.Time = firstP.Time;
                    newPS.DistanceMeter = firstP.DistanceMeter;
                    newPS.Position.DeltaDistanceMeters = firstP.Position.DeltaDistanceMeters;
                    newPS.Position.DeltaAltitudeMeters = firstP.Position.DeltaAltitudeMeters;
                    newPS.ElapsedTime = new TimeSpan(0);

                    newPoints.Insert(0, newPS);
                    #endregion
                }

                return new CyclingData(newPoints.ToArray(), this);
            }

            return this;
        }

        private void AddPointsToResult(ref List<CyclingDataPoint> resultCollection, ref List<CyclingDataPoint> tempCollection, bool isInsertToFirst = false)
        {
            var lastNewP = this.GetMaxNPowerDataPoint(tempCollection.ToArray());

            #region add blank last P to make trendLine flat
            var lastP = tempCollection.Last();

            var newP = this.GetMaxNPowerDataPoint(new CyclingDataPoint[] { lastNewP });
            newP.Time = lastP.Time;
            newP.DistanceMeter = lastP.DistanceMeter;
            newP.Position.DeltaAltitudeMeters = 0;
            newP.Position.DeltaDistanceMeters = double.NaN;
            newP.Position.LatitudeDegrees = lastP.Position.LatitudeDegrees;
            newP.Position.LongitudeDegrees = lastP.Position.LongitudeDegrees;
            newP.Position.AltitudeMeters = lastP.Position.AltitudeMeters;
            newP.ElapsedTime = new TimeSpan(0);

            if (isInsertToFirst)
            {
                resultCollection.Insert(0, newP);
                resultCollection.Insert(0, lastNewP);
            }
            else
            {
                resultCollection.Add(lastNewP);
                resultCollection.Add(newP);
            }
            #endregion

            tempCollection.Clear();
        }

        public void UpdateParameters(int durationSeconds, int slopeMeter, int slopeMinSeconds, bool calculateStand, bool calculateSeat)
        {
            this.isCalculateStand = calculateStand;
            this.isCalculateSeat = calculateSeat;

            if (this._DataPoints.Count > 0)
            {
                if (durationSeconds > 1)
                {
                    var tempData = GetMaxNPowerBasedSplitData(durationSeconds);
                    this._DataPoints = tempData.DataPoints;
                    //this._Laps = tempData.Laps;
                }
            }

            this._SlopeSplitMeter = slopeMeter;
            this._SlopeSplitSeconds = slopeMinSeconds;

            this.GetSlopeData(this._SlopeSplitMeter);
        }

        private double GetSummaryAngle(double angle, bool isEnd = false)
        {
            if (angle >= 180 && !isEnd)
            {
                angle = angle - 360;
            }

            return angle;
        }

        private double GetFixedAngle(double angle)
        {
            if (angle < 0)
            {
                angle = angle + 360;
            }

            return angle;
        }
        #endregion
    }
}
