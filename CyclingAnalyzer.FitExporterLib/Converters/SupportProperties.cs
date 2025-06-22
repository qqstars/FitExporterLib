using CyclingAnalyzer.FitExporterLib.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporterLib.Converters
{
    public static class SupportProperties
    {
        public const string Time = "Time";

        public const string ElapsedTimeSeconds = "ElapsedTime";

        public const string PositionLatitude = "PositionLatitudeDegrees";

        public const string PositionLongitude = "PositionLongitudeDegrees";

        public const string Altitude = "AltitudeMeters";

        public const string Distance = "DistanceMeter";

        public const string Slope = "Slope";

        public const string Speed = "Speed";

        public const string HeartRate = "HeartRateBpm";

        public const string Cadence = "CadenceRpm";

        public const string Power = "Power";

        public const string Temperature = "Temperature";

        public const string OneSecChangeRateSuffix = "OneSecChangeRate";

        public const string ThreeSecChangeRateSuffix = "ThreeSecChangeRate";

        public static string[] AllProperties = new string[]
        {
            Time,
            ElapsedTimeSeconds,
            Distance,
            Slope,
            Altitude,
            Speed,
            HeartRate,
            Cadence,
            Power,
            Temperature
        };

        public static string[] AllChangeRateSupportedProperties = new string[]
        {
            Altitude,
            Speed,
            HeartRate,
            Cadence,
            Power
        };

        public static string GetPropertyValue(this CyclingDataPoint point, string propertyName, CyclingDataPoint? pointOneSecBefore = null, CyclingDataPoint? pointThreeSecBefore = null)
        {
            switch (propertyName)
            {
                case Time:
                    return point.Time.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
                case ElapsedTimeSeconds:
                    return point.ElapsedTime.TotalSeconds.ToString();
                case PositionLatitude:
                    return point.Position.LatitudeDegrees.ToString();
                case PositionLongitude:
                    return point.Position.LongitudeDegrees.ToString();
                case Distance:
                    return point.DistanceMeter.ToString("F2");
                case Slope:
                    return point.Position.MomentSlope.ToString("F6");
                case Altitude:
                    return point.Position.AltitudeMeters.ToString("F2");
                case Altitude + OneSecChangeRateSuffix:
                    return pointOneSecBefore != null && (point.Time - pointOneSecBefore.Time).TotalSeconds > 0 ? ((point.Position.AltitudeMeters - pointOneSecBefore.Position.AltitudeMeters) / (point.Time - pointOneSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Altitude + ThreeSecChangeRateSuffix:
                    return pointThreeSecBefore != null && (point.Time - pointThreeSecBefore.Time).TotalSeconds > 0 ? ((point.Position.AltitudeMeters - pointThreeSecBefore.Position.AltitudeMeters) / (point.Time - pointThreeSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Speed:
                    return point.Speed.ToString("F2");
                case Speed + OneSecChangeRateSuffix:
                    return pointOneSecBefore != null && (point.Time - pointOneSecBefore.Time).TotalSeconds > 0 ? ((point.Speed - pointOneSecBefore.Speed) / (point.Time - pointOneSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Speed + ThreeSecChangeRateSuffix:
                    return pointThreeSecBefore != null && (point.Time - pointThreeSecBefore.Time).TotalSeconds > 0 ? ((point.Speed - pointThreeSecBefore.Speed) / (point.Time - pointThreeSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case HeartRate:
                    return point.HeartRateBpm.ToString();
                case HeartRate + OneSecChangeRateSuffix:
                    return pointOneSecBefore != null && (point.Time - pointOneSecBefore.Time).TotalSeconds > 0 ? ((point.HeartRateBpm - pointOneSecBefore.HeartRateBpm) / (point.Time - pointOneSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case HeartRate + ThreeSecChangeRateSuffix:
                    return pointThreeSecBefore != null && (point.Time - pointThreeSecBefore.Time).TotalSeconds > 0 ? ((point.HeartRateBpm - pointThreeSecBefore.HeartRateBpm) / (point.Time - pointThreeSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Cadence:
                    return point.Cadence.ToString();
                case Cadence + OneSecChangeRateSuffix:
                    return pointOneSecBefore != null && (point.Time - pointOneSecBefore.Time).TotalSeconds > 0 ? ((point.Cadence - pointOneSecBefore.Cadence) / (point.Time - pointOneSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Cadence + ThreeSecChangeRateSuffix:
                    return pointThreeSecBefore != null && (point.Time - pointThreeSecBefore.Time).TotalSeconds > 0 ? ((point.Cadence - pointThreeSecBefore.Cadence) / (point.Time - pointThreeSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Power:
                    return point.Power.TotalPower.ToString();
                case Power + OneSecChangeRateSuffix:
                    return pointOneSecBefore != null && (point.Time - pointOneSecBefore.Time).TotalSeconds > 0 ? ((point.Power.TotalPower - pointOneSecBefore.Power.TotalPower) / (point.Time - pointOneSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Power + ThreeSecChangeRateSuffix:
                    return pointThreeSecBefore != null && (point.Time - pointThreeSecBefore.Time).TotalSeconds > 0 ? ((point.Power.TotalPower - pointThreeSecBefore.Power.TotalPower) / (point.Time - pointThreeSecBefore.Time).TotalSeconds).ToString("F3") : "0";
                case Temperature:
                    return point.Temperature.ToString("F2");
                default:
                    throw new ArgumentException($"Property '{propertyName}' is not supported.");
            }
        }
    }
}
