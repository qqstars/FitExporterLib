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

        public const string Speed = "Speed";

        public const string HeartRate = "HeartRateBpm";

        public const string Cadence = "CadenceRpm";

        public const string Power = "Power";

        public const string Temperature = "Temperature";

        public static string[] AllProperties = new string[]
        {
            Time,
            ElapsedTimeSeconds,
            Distance,
            Altitude,
            Speed,
            HeartRate,
            Cadence,
            Power,
            Temperature
        };

        public static string GetPropertyValue(this CyclingDataPoint point, string propertyName)
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
                case Altitude:
                    return point.Position.AltitudeMeters.ToString("F2");
                case Distance:
                    return point.DistanceMeter.ToString("F2");
                case Speed:
                    return point.Speed.ToString("F2");
                case HeartRate:
                    return point.HeartRateBpm.ToString();
                case Cadence:
                    return point.Cadence.ToString();
                case Power:
                    return point.Power.TotalPower.ToString();
                case Temperature:
                    return point.Temperature.ToString("F2");
                default:
                    throw new ArgumentException($"Property '{propertyName}' is not supported.");
            }
        }
    }
}
