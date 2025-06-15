using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace CyclingAnalyzer.FitExporterLib.DataModel
{
    public static class DataRangeHelper
    {
        public static void GetHRRange(double hrLT, out double bondary1, out double bondary2, out double bondary3, out double bondary4, out double bondary5, out double bondary6, out double bondary7)
        {
            if (30 >= hrLT)
            {
                bondary1 = bondary2 = bondary3 = bondary4 = bondary5 = bondary6 = bondary7 = 0;
                return;
            }

            #region matrix
            double[,] hrMatrix = 
            { 
                {108,122,128,136,140,145,150},
                {109,123,129,137,141,146,151},
                {109,124,130,138,142,147,152},
                {110,125,130,139,143,147,153},
                {111,125,131,140,144,148,154},

                {112,126,132,141,145,149,155},
                {112,127,133,142,145,150,156},
                {113,128,134,143,147,151,157},
                {114,129,135,144,148,152,158},
                {115,130,136,145,149,154,159},

                {116,131,137,146,150,155,161},
                {117,132,138,147,151,156,162},
                {118,133,139,148,152,157,163},
                {119,134,140,149,153,158,164},
                {120,134,141,150,154,159,165},

                {121,135,142,151,155,160,166},
                {122,136,142,152,156,161,167},
                {123,137,143,153,157,162,168},
                {124,138,144,154,158,163,169},
                {125,138,145,155,159,164,170},

                {126,140,146,156,160,165,171},
                {127,141,147,157,161,167,173},
                {128,142,148,158,162,168,174},
                {129,143,148,159,163,169,175},
                {129,143,150,160,164,170,176},

                {130,144,151,161,165,171,177},
                {131,145,152,162,166,172,178},
                {132,146,153,163,167,173,179},
                {133,147,154,164,168,174,180},
                {134,148,154,165,169,175,181},

                {135,149,155,166,170,176,182},
                {136,150,156,167,171,177,183},
                {137,151,157,168,172,178,185},
                {138,151,158,169,173,179,186},
                {139,152,160,170,174,180,187},

                {140,153,160,171,175,181,188},
                {141,154,161,172,176,182,189},
                {142,155,162,173,177,183,190},
                {143,156,163,174,178,184,191},
                {144,157,164,175,179,185,192},

                {145,158,165,176,180,186,193},
                {146,159,166,177,181,187,194},
                {147,160,166,178,182,188,195},
                {148,160,167,179,183,190,197},
                {149,161,168,180,184,191,198},

                {150,162,170,181,185,192,199},
                {151,163,171,182,186,193,200},
                {152,164,172,183,187,194,201},
                {153,165,172,184,188,195,202},
                {154,166,173,185,189,196,203},

                {155,167,174,186,190,197,204},
                {156,168,175,187,191,198,205},
                {157,169,176,188,192,199,206},
                {158,170,177,189,193,200,207},
                {159,170,178,190,194,201,208},

                {160,171,178,191,195,202,209},
                {161,172,179,192,196,203,210},
                {162,173,180,193,197,204,211},
                {163,174,181,194,198,205,212},
                {164,175,182,195,199,206,213}
            };
            #endregion

            bondary1 = (int)(0.808 * hrLT + 0.5);
            bondary2 = (int)(0.892 * hrLT + 0.5);
            bondary3 = (int)(0.928 * hrLT + 0.5);
            bondary4 = ((int)hrLT) - 1;
            bondary5 = ((int)hrLT) + 3;
            bondary6 = ((int)hrLT) + 9;
            bondary7 = ((int)hrLT) + 15;

            if (hrMatrix[0, 3] >= hrLT)
            {
                return;
            }

            for(int i = 0; i< hrMatrix.GetLength(0);i++)
            {
                if (hrMatrix[i, 3] + 1 >= hrLT)
                {
                    bondary1 = hrMatrix[i, 0];
                    bondary2 = hrMatrix[i, 1];
                    bondary3 = hrMatrix[i, 2];
                    bondary4 = hrMatrix[i, 3];
                    bondary5 = hrMatrix[i, 4];
                    bondary6 = hrMatrix[i, 5];
                    bondary7 = hrMatrix[i, 6];

                    break;
                }
            }
        }

        public static void GetPowerRange(double ftp, out double bondary1, out double bondary2, out double bondary3, out double bondary4, out double bondary5, out double bondary6)
        {
            bondary1 = 0.55 * ftp;
            bondary2 = 0.75 * ftp;
            bondary3 = 0.9 * ftp;
            bondary4 = 1.05 * ftp;
            bondary5 = 1.2 * ftp;
            bondary6 = 1.5 * ftp;
        }

        public static void GetRangeData(this CyclingData data, double ltHR, double ftp)
        {
            GetHRRangeData(ref data, ltHR);

            GetPowerRangeData(ref data, ftp);
        }

        public static void GetHRRangeData(ref CyclingData data, double hrLT)
        {
            double boundary1, boundary2, boundary3, boundary4, boundary5, boundary6, boundary7;

            GetHRRange(hrLT, out boundary1, out boundary2, out boundary3, out boundary4, out boundary5, out boundary6, out boundary7);


            if (null == data.DataPoints || 0 >= data.DataPoints.Count 
                || 0 >= boundary1 || 0 >= boundary2 || 0 >= boundary3 || 0 >= boundary4 || 0 >= boundary5 || 0 >= boundary6 || 0 >= boundary7)
            {
                return;
            }

            #region set boundary
            data.HeartRateRangeData.Range1Recovery.LowerBoundary = 0;
            data.HeartRateRangeData.Range1Recovery.UpperBoundary = boundary1;

            data.HeartRateRangeData.Range2Endurance.LowerBoundary = boundary1;
            data.HeartRateRangeData.Range2Endurance.UpperBoundary = boundary2;

            data.HeartRateRangeData.Range3Tempo.LowerBoundary = boundary2;
            data.HeartRateRangeData.Range3Tempo.UpperBoundary = boundary3;

            data.HeartRateRangeData.Range4LessThreshold.LowerBoundary = boundary3;
            data.HeartRateRangeData.Range4LessThreshold.UpperBoundary = boundary4;

            data.HeartRateRangeData.Range5aMoreThreshold.LowerBoundary = boundary4;
            data.HeartRateRangeData.Range5aMoreThreshold.UpperBoundary = boundary5;

            data.HeartRateRangeData.Range5bVO2Max.LowerBoundary = boundary5;
            data.HeartRateRangeData.Range5bVO2Max.UpperBoundary = boundary6;

            data.HeartRateRangeData.Range5cSprint.LowerBoundary = boundary6;
            data.HeartRateRangeData.Range5cSprint.UpperBoundary = boundary7;
            #endregion

            #region Get seconds
            data.HeartRateRangeData.Range1Recovery.TotalSeconds = 0;
            data.HeartRateRangeData.Range2Endurance.TotalSeconds = 0;
            data.HeartRateRangeData.Range3Tempo.TotalSeconds = 0;
            data.HeartRateRangeData.Range4LessThreshold.TotalSeconds = 0;
            data.HeartRateRangeData.Range5aMoreThreshold.TotalSeconds = 0;
            data.HeartRateRangeData.Range5bVO2Max.TotalSeconds = 0;
            data.HeartRateRangeData.Range5cSprint.TotalSeconds = 0;

            foreach (CyclingDataPoint point in data.DataPoints)
            {
                if (
                    data.HeartRateRangeData.Range1Recovery.LowerBoundary <= point.HeartRateBpm
                    && data.HeartRateRangeData.Range1Recovery.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range1Recovery.TotalSeconds++;
                }
                else if (
                    data.HeartRateRangeData.Range2Endurance.LowerBoundary < point.HeartRateBpm
                    && data.HeartRateRangeData.Range2Endurance.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range2Endurance.TotalSeconds++;
                }
                else if (
                    data.HeartRateRangeData.Range3Tempo.LowerBoundary < point.HeartRateBpm
                    && data.HeartRateRangeData.Range3Tempo.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range3Tempo.TotalSeconds++;
                }
                else if (
                    data.HeartRateRangeData.Range4LessThreshold.LowerBoundary < point.HeartRateBpm
                    && data.HeartRateRangeData.Range4LessThreshold.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range4LessThreshold.TotalSeconds++;
                }
                else if (
                    data.HeartRateRangeData.Range5aMoreThreshold.LowerBoundary < point.HeartRateBpm
                    && data.HeartRateRangeData.Range5aMoreThreshold.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range5aMoreThreshold.TotalSeconds++;
                }
                else if (
                    data.HeartRateRangeData.Range5bVO2Max.LowerBoundary < point.HeartRateBpm
                    && data.HeartRateRangeData.Range5bVO2Max.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range5bVO2Max.TotalSeconds++;
                }
                else if (
                    data.HeartRateRangeData.Range5cSprint.LowerBoundary < point.HeartRateBpm
                    && data.HeartRateRangeData.Range5cSprint.UpperBoundary >= point.HeartRateBpm
                    )
                {
                    data.HeartRateRangeData.Range5cSprint.TotalSeconds++;
                }
            }
            #endregion

            #region Get percentage
            data.HeartRateRangeData.Range1Recovery.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range1Recovery.TotalSeconds / data.DataPoints.Count;
            data.HeartRateRangeData.Range2Endurance.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range2Endurance.TotalSeconds / data.DataPoints.Count;
            data.HeartRateRangeData.Range3Tempo.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range3Tempo.TotalSeconds / data.DataPoints.Count;
            data.HeartRateRangeData.Range4LessThreshold.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range4LessThreshold.TotalSeconds / data.DataPoints.Count;
            data.HeartRateRangeData.Range5aMoreThreshold.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range5aMoreThreshold.TotalSeconds / data.DataPoints.Count;
            data.HeartRateRangeData.Range5bVO2Max.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range5bVO2Max.TotalSeconds / data.DataPoints.Count;
            data.HeartRateRangeData.Range5cSprint.TotalSecondsPercentage = 100 * data.HeartRateRangeData.Range5cSprint.TotalSeconds / data.DataPoints.Count;
            #endregion
        }

        public static void GetPowerRangeData(ref CyclingData data, double ftp)
        {
            GetPowerRange(ftp, out double boundary1, out double boundary2, out double boundary3, out double boundary4, out double boundary5, out double boundary6);
            GetPowerRangeData(ref data, boundary1, boundary2, boundary3, boundary4, boundary5, boundary6);
        }

        public static void GetPowerRangeData(ref CyclingData data, double boundary1, double boundary2, double boundary3, double boundary4, double boundary5, double boundary6)
        {
            if (null == data.DataPoints || 0 >= data.DataPoints.Count || 0 >= boundary1 || 0 >= boundary2 || 0 >= boundary3 || 0 >= boundary4 || 0 >= boundary5 || 0 >= boundary6)
            {
                return;
            }

            #region set boundary
            data.PowerRangeData.Range1Recovery.LowerBoundary = 0;
            data.PowerRangeData.Range1Recovery.UpperBoundary = boundary1;

            data.PowerRangeData.Range2Endurance.LowerBoundary = boundary1;
            data.PowerRangeData.Range2Endurance.UpperBoundary = boundary2;

            data.PowerRangeData.Range3Tempo.LowerBoundary = boundary2;
            data.PowerRangeData.Range3Tempo.UpperBoundary = boundary3;

            data.PowerRangeData.Range4Threshold.LowerBoundary = boundary3;
            data.PowerRangeData.Range4Threshold.UpperBoundary = boundary4;

            data.PowerRangeData.Range5VO2Max.LowerBoundary = boundary4;
            data.PowerRangeData.Range5VO2Max.UpperBoundary = boundary5;

            data.PowerRangeData.Range6AnaerobicCapacity.LowerBoundary = boundary5;
            data.PowerRangeData.Range6AnaerobicCapacity.UpperBoundary = boundary6;

            data.PowerRangeData.Range7Sprint.LowerBoundary = boundary6;
            data.PowerRangeData.Range7Sprint.UpperBoundary = int.MaxValue;
            #endregion

            #region Get seconds
            data.PowerRangeData.Range1Recovery.TotalSeconds = 0;
            data.PowerRangeData.Range2Endurance.TotalSeconds = 0;
            data.PowerRangeData.Range3Tempo.TotalSeconds = 0;
            data.PowerRangeData.Range4Threshold.TotalSeconds = 0;
            data.PowerRangeData.Range5VO2Max.TotalSeconds = 0;
            data.PowerRangeData.Range6AnaerobicCapacity.TotalSeconds = 0;
            data.PowerRangeData.Range7Sprint.TotalSeconds = 0;

            foreach (CyclingDataPoint point in data.DataPoints)
            {
                if (
                    data.PowerRangeData.Range1Recovery.LowerBoundary <= point.Power.TotalPower
                    && data.PowerRangeData.Range1Recovery.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range1Recovery.TotalSeconds++;
                }
                else if (
                    data.PowerRangeData.Range2Endurance.LowerBoundary < point.Power.TotalPower
                    && data.PowerRangeData.Range2Endurance.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range2Endurance.TotalSeconds++;
                }
                else if (
                    data.PowerRangeData.Range3Tempo.LowerBoundary < point.Power.TotalPower
                    && data.PowerRangeData.Range3Tempo.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range3Tempo.TotalSeconds++;
                }
                else if (
                    data.PowerRangeData.Range4Threshold.LowerBoundary < point.Power.TotalPower
                    && data.PowerRangeData.Range4Threshold.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range4Threshold.TotalSeconds++;
                }
                else if (
                    data.PowerRangeData.Range5VO2Max.LowerBoundary < point.Power.TotalPower
                    && data.PowerRangeData.Range5VO2Max.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range5VO2Max.TotalSeconds++;
                }
                else if (
                    data.PowerRangeData.Range6AnaerobicCapacity.LowerBoundary < point.Power.TotalPower
                    && data.PowerRangeData.Range6AnaerobicCapacity.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range6AnaerobicCapacity.TotalSeconds++;
                }
                else if (
                    data.PowerRangeData.Range7Sprint.LowerBoundary < point.Power.TotalPower
                    && data.PowerRangeData.Range7Sprint.UpperBoundary >= point.Power.TotalPower
                    )
                {
                    data.PowerRangeData.Range7Sprint.TotalSeconds++;
                }
            }
            #endregion

            #region Get percentage
            data.PowerRangeData.Range1Recovery.TotalSecondsPercentage = 100 * data.PowerRangeData.Range1Recovery.TotalSeconds / data.DataPoints.Count;
            data.PowerRangeData.Range2Endurance.TotalSecondsPercentage = 100 * data.PowerRangeData.Range2Endurance.TotalSeconds / data.DataPoints.Count;
            data.PowerRangeData.Range3Tempo.TotalSecondsPercentage = 100 * data.PowerRangeData.Range3Tempo.TotalSeconds / data.DataPoints.Count;
            data.PowerRangeData.Range4Threshold.TotalSecondsPercentage = 100 * data.PowerRangeData.Range4Threshold.TotalSeconds / data.DataPoints.Count;
            data.PowerRangeData.Range5VO2Max.TotalSecondsPercentage = 100 * data.PowerRangeData.Range5VO2Max.TotalSeconds / data.DataPoints.Count;
            data.PowerRangeData.Range6AnaerobicCapacity.TotalSecondsPercentage = 100 * data.PowerRangeData.Range6AnaerobicCapacity.TotalSeconds / data.DataPoints.Count;
            data.PowerRangeData.Range7Sprint.TotalSecondsPercentage = 100 * data.PowerRangeData.Range7Sprint.TotalSeconds / data.DataPoints.Count;
            #endregion
        }
    }

    public struct RangeStruct
    {
        public double LowerBoundary;
        public double UpperBoundary;

        public double TotalSeconds;
        public double TotalSecondsPercentage;
    }

    public struct PowerRange
    {
        public RangeStruct Range1Recovery;
        public RangeStruct Range2Endurance;
        public RangeStruct Range3Tempo;
        public RangeStruct Range4Threshold;
        public RangeStruct Range5VO2Max;
        public RangeStruct Range6AnaerobicCapacity;
        public RangeStruct Range7Sprint;
    }

    public struct HeartRateRange
    {
        public RangeStruct Range1Recovery;
        public RangeStruct Range2Endurance;
        public RangeStruct Range3Tempo;
        public RangeStruct Range4LessThreshold;
        public RangeStruct Range5aMoreThreshold;
        public RangeStruct Range5bVO2Max;
        public RangeStruct Range5cSprint;
    }
}
