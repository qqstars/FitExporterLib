using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporterLib.DataModel.Types
{
    public class PowerType
    {
        private double _TotalPower = -1;

        /// <summary>
        /// Total power (W)
        /// </summary>
        public double TotalPower
        {
            get
            {
                if (_TotalPower < 0) _TotalPower = 0;
                return _TotalPower > 0 ? _TotalPower : _LeftLegPower + _RightLegPower;
            }
            set { _TotalPower = value; }
        }


        private double _LeftLegPower = 0;

        /// <summary>
        /// Left power (W)
        /// </summary>
        public double LeftLegPower
        {
            get { if (_LeftLegPower < 0) _LeftLegPower = 0; return _LeftLegPower; }
            set { _LeftLegPower = value; }
        }

        private double _RightLegPower = 0;

        /// <summary>
        /// Right power (W)
        /// </summary>
        public double RightLegPower
        {
            get { if (_RightLegPower < 0) _RightLegPower = 0; return _RightLegPower; }
            set { _RightLegPower = value; }
        }

        /// <summary>
        /// Left leg percentage. 48 means 48%
        /// </summary>
        public double LeftLegPercentage
        {
            get
            {
                if (LeftLegPower > 0)
                {
                    return LeftLegPower * 100 / TotalPower;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Right leg percentage. 52 means 52%
        /// </summary>
        public double RightLegPercentage
        {
            get
            {
                if (RightLegPower > 0)
                {
                    return RightLegPower * 100 / TotalPower;
                }
                else
                {
                    return 0;
                }
            }
        }

        private double _AccumulatedPower = -1;

        /// <summary>
        /// 累计功率
        /// </summary>
        public double AccumulatedPower
        {
            get { return _AccumulatedPower; }
            set { _AccumulatedPower = value; }
        }

        private double _LeftTorqueEffectiveness = -1;

        /// <summary>
        /// 左腿扭矩效益
        /// </summary>
        public double LeftTorqueEffectiveness
        {
            get { return _LeftTorqueEffectiveness; }
            set { _LeftTorqueEffectiveness = value; }
        }

        private double _RightTorqueEffectiveness = -1;

        /// <summary>
        /// 右腿扭矩效益
        /// </summary>
        public double RightTorqueEffectiveness
        {
            get { return _RightTorqueEffectiveness; }
            set { _RightTorqueEffectiveness = value; }
        }

        private double _TorqueEffectiveness = -1;

        /// <summary>
        /// 平均扭矩效益
        /// </summary>
        public double TorqueEffectiveness
        {
            get
            {
                if (0 < _TorqueEffectiveness)
                {
                    return _TorqueEffectiveness;
                }
                else
                {
                    return (LeftTorqueEffectiveness + RightTorqueEffectiveness) / 2;
                }
            }
            set { _TorqueEffectiveness = value; }
        }

        private double _LeftPedalSmoothness = -1;

        /// <summary>
        /// 左腿踩踏平滑
        /// </summary>
        public double LeftPedalSmoothness
        {
            get { return _LeftPedalSmoothness; }
            set { _LeftPedalSmoothness = value; }
        }

        private double _RightPedalSmoothness = -1;

        /// <summary>
        /// 右腿踩踏平滑
        /// </summary>
        public double RightPedalSmoothness
        {
            get { return _RightPedalSmoothness; }
            set { _RightPedalSmoothness = value; }
        }

        private double _PedalSmoothness = -1;

        /// <summary>
        /// 平均踩踏平滑
        /// </summary>
        public double PedalSmoothness
        {
            get
            {
                if (0 < _PedalSmoothness)
                {
                    return _PedalSmoothness;
                }
                else
                {
                    return (LeftPedalSmoothness + RightPedalSmoothness) / 2;
                }
            }
            set { _PedalSmoothness = value; }
        }

        private double _LeftPowerPhaseStart = double.NaN;

        public double LeftPowerPhaseStart
        {
            get { return _LeftPowerPhaseStart; }
            set { _LeftPowerPhaseStart = value; }
        }
        private double _LeftPowerPhaseEnd = double.NaN;

        public double LeftPowerPhaseEnd
        {
            get { return _LeftPowerPhaseEnd; }
            set { _LeftPowerPhaseEnd = value; }
        }

        private double _LeftPowerPhasePeakStart = double.NaN;

        public double LeftPowerPhasePeakStart
        {
            get { return _LeftPowerPhasePeakStart; }
            set { _LeftPowerPhasePeakStart = value; }
        }
        private double _LeftPowerPhasePeakEnd = double.NaN;

        public double LeftPowerPhasePeakEnd
        {
            get { return _LeftPowerPhasePeakEnd; }
            set { _LeftPowerPhasePeakEnd = value; }
        }

        private double _LeftPlatformCenterOffset = 0.0;

        public double LeftPlatformCenterOffset
        {
            get { return _LeftPlatformCenterOffset; }
            set { _LeftPlatformCenterOffset = value; }
        }

        private double _RightPowerPhaseStart = double.NaN;

        public double RightPowerPhaseStart
        {
            get { return _RightPowerPhaseStart; }
            set { _RightPowerPhaseStart = value; }
        }
        private double _RightPowerPhaseEnd = double.NaN;

        public double RightPowerPhaseEnd
        {
            get { return _RightPowerPhaseEnd; }
            set { _RightPowerPhaseEnd = value; }
        }

        private double _RightPowerPhasePeakStart = double.NaN;

        public double RightPowerPhasePeakStart
        {
            get { return _RightPowerPhasePeakStart; }
            set { _RightPowerPhasePeakStart = value; }
        }
        private double _RightPowerPhasePeakEnd = double.NaN;

        public double RightPowerPhasePeakEnd
        {
            get { return _RightPowerPhasePeakEnd; }
            set { _RightPowerPhasePeakEnd = value; }
        }

        private double _RightPlatformCenterOffset = 0.0;

        public double RightPlatformCenterOffset
        {
            get { return _RightPlatformCenterOffset; }
            set { _RightPlatformCenterOffset = value; }
        }

        private double _StanceTime = 0.0;

        /// <summary>
        /// Stance time. xx ms
        /// </summary>
        public double StanceTime
        {
            get { return _StanceTime; }
            set { _StanceTime = value; }
        }

        public bool IsStand
        {
            get { return _StanceTime > 0; }
        }
    }
}
