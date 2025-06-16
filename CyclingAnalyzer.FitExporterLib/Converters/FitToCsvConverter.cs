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
        IDictionary<string, string> additionalPropertiesValues = null;

        public FitToCsvConverter(IEnumerable<string> includeProperties = null, IDictionary<string, string> additionalPropertiesValues = null)
        {
            this.includeProperties = includeProperties ?? SupportProperties.AllProperties;
            this.additionalPropertiesValues = additionalPropertiesValues ?? new Dictionary<string, string>();
        }

        public string ConvertToCsvString(CyclingData data)
        {
            StringBuilder csvBuilder = new StringBuilder();
            // Add header
            var header = string.Join(",", additionalPropertiesValues.Keys) + (additionalPropertiesValues.Count > 0 ? "," : string.Empty) + string.Join(",", includeProperties);
            csvBuilder.AppendLine(header);

            // Add data points
            foreach (var point in data.DataPoints)
            {
                var values = additionalPropertiesValues.Values
                    .Concat(includeProperties.Select(prop => point.GetPropertyValue(prop)));

                csvBuilder.AppendLine(string.Join(",", values));
            }

            return csvBuilder.ToString();
        }

        public bool ConvertToCsvFile(string fitFilePath, string csvFilePath)
        {
            if (GarminFitReader.ReadFile(fitFilePath, out GarminFitValidation isValidFile, out CyclingData cyclingData, out DateTime firstPointDateTime)
                && isValidFile == GarminFitValidation.ValidFile)
            {
                if (File.Exists(csvFilePath))
                {
                    File.Delete(csvFilePath);
                }

                File.WriteAllText(csvFilePath, this.ConvertToCsvString(cyclingData), Encoding.UTF8);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
