using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyclingAnalyzer.FitExporterLib.DataModel;
using CyclingAnalyzer.FitExporterLib.GarminFit;

namespace CyclingAnalyzer.FitExporterLib.Converters
{
    public class FitToCsvConverter
    {
        IEnumerable<string> includeProperties;
        IDictionary<string, string> additionalPropertiesValues;

        public FitToCsvConverter(IEnumerable<string>? includeProperties = null, IDictionary<string, string>? additionalPropertiesValues = null)
        {
            this.includeProperties = includeProperties ?? SupportProperties.AllProperties;
            this.additionalPropertiesValues = additionalPropertiesValues ?? new Dictionary<string, string>();
        }

        public string ConvertToCsvString(CyclingData data, bool containsOneSecChangeRate = false, bool containsThreeSecChangeRate = false)
        {
            StringBuilder csvBuilder = new StringBuilder();

            var includeProps = GetIncludeProperties(this.includeProperties, containsOneSecChangeRate, containsThreeSecChangeRate);

            // Add header
            var header = string.Join(",", additionalPropertiesValues.Keys) + (additionalPropertiesValues.Count > 0 ? "," : string.Empty) + string.Join(",", includeProps);
            csvBuilder.AppendLine(header);

            // Add data points
            for (var i = 0; i < data.DataPoints.Count; i++)
            {
                CyclingDataPoint point = data.DataPoints[i];
                CyclingDataPoint? pointOneSecBefore = null;
                CyclingDataPoint? pointThreeSecBefore = null;

                if (i >= 1)
                {
                    pointOneSecBefore = data.DataPoints[i - 1];
                }

                if (i >= 3)
                {
                    pointThreeSecBefore = data.DataPoints[i - 3];
                }

                var values = additionalPropertiesValues.Values
                    .Concat(includeProps.Select(prop => point.GetPropertyValue(prop, pointOneSecBefore, pointThreeSecBefore)));

                csvBuilder.AppendLine(string.Join(",", values));
            }

            return csvBuilder.ToString();
        }

        public bool ConvertToCsvFile(string fitFilePath, string csvFilePath, bool containsOneSecChangeRate = false, bool containsThreeSecChangeRate = false)
        {
            if (GarminFitReader.ReadFile(fitFilePath, out GarminFitValidation isValidFile, out CyclingData cyclingData, out DateTime firstPointDateTime)
                && isValidFile == GarminFitValidation.ValidFile)
            {
                if (File.Exists(csvFilePath))
                {
                    File.Delete(csvFilePath);
                }

                File.WriteAllText(csvFilePath, this.ConvertToCsvString(cyclingData, containsOneSecChangeRate, containsThreeSecChangeRate), Encoding.UTF8);

                return true;
            }
            else
            {
                return false;
            }
        }

        private static IEnumerable<string> GetIncludeProperties(IEnumerable<string> includeProperties, bool containsOneSecChangeRate, bool containsThreeSecChangeRate)
        {
            var results = new List<string>();

            foreach (string prop in includeProperties)
            {
                results.Add(prop);

                if (!string.Equals(prop, SupportProperties.Time, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(prop, SupportProperties.ElapsedTimeSeconds, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(prop, SupportProperties.PositionLatitude, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(prop, SupportProperties.PositionLongitude, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(prop, SupportProperties.Distance, StringComparison.OrdinalIgnoreCase)
                    && !string.Equals(prop, SupportProperties.Temperature, StringComparison.OrdinalIgnoreCase))
                {
                    if (containsOneSecChangeRate)
                    {
                        results.Add(prop + SupportProperties.OneSecChangeRateSuffix);
                    }

                    if (containsThreeSecChangeRate)
                    {
                        results.Add(prop + SupportProperties.ThreeSecChangeRateSuffix);
                    }
                }
            }

            var resultArr = results.ToArray();
            results.Clear();

            return resultArr;
        }
    }
}
