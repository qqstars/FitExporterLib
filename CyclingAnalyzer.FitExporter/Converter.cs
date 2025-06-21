using CyclingAnalyzer.FitExporterLib.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporter
{
    internal class Converter
    {
        public static void Convert(Options options)
        {
            var inputPath = options.InputPath;
            var outputPath = options.OutputPath;

            Convert(options.InputPath, options.OutputPath, options.OutputType, options.DirectoryNameFormat, options.ContainsOneSecChangeRate, options.ContainsThreeSecChangeRate);
        }

        public static void Convert(string inputPath, string outputPath, OutputType outputType, string directoryNameFormat = "", bool containsOneSecChangeRate = false, bool containsThreeSecChangeRate = false)
        {
            
            if (File.Exists(inputPath))
            {
                // input path is a file, directly convert it.
                var fileAdditionalPropertyValues = ConverterHelpers.GetMapValues(directoryNameFormat, Path.GetFileNameWithoutExtension(Path.GetDirectoryName(inputPath)));

                Console.WriteLine($"Converting single file: {inputPath} to {outputPath} with output type {outputType}.");
                Console.WriteLine($"Additional Properties:");
                Console.WriteLine();
                Console.WriteLine(string.Join("\r\n", fileAdditionalPropertyValues.Select(kvp => $"{kvp.Key}: {kvp.Value}")));

                ConvertFile(inputPath, outputPath, outputType, fileAdditionalPropertyValues, containsOneSecChangeRate, containsThreeSecChangeRate);
                return;
            }

            // else, if the input path is a directory, iterate all sub-folders and files under the path.
            var directoryInfo = new DirectoryInfo(inputPath);
            IDictionary<string, string> additionalPropertyValues = ConverterHelpers.GetMapValues(directoryNameFormat, Path.GetFileNameWithoutExtension(inputPath));

            Console.WriteLine("---------------------------------- Folder ----------------------------------");
            Console.WriteLine($"Converting files in directory: {directoryInfo.FullName} to {outputPath} with output type {outputType}.");
            Console.WriteLine($"Additional Properties:");
            Console.WriteLine();
            Console.WriteLine(string.Join("\r\n", additionalPropertyValues.Select(kvp => $"{kvp.Key}: {kvp.Value}")));

            // Get the directories BEFORE generating files, because the files might be generated in the same directory.
            var directories = directoryInfo.GetDirectories();

            foreach (var file in directoryInfo.GetFiles())
            {
                if (string.Equals(".dll", file.Extension, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(".exe", file.Extension, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(".pdb", file.Extension, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (ConvertFile(file.FullName, outputPath, outputType, additionalPropertyValues, containsOneSecChangeRate, containsThreeSecChangeRate))
                { 
                    Console.WriteLine($"Converted {file.FullName} to {outputPath} successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to convert {file.FullName}.");
                }
            }

            foreach (var subDirectory in directories)
            {
                // iterate all sub-directories and convert them recursively.
                Convert(subDirectory.FullName, Path.Combine(outputPath, Path.GetFileNameWithoutExtension(subDirectory.FullName)), outputType, directoryNameFormat, containsOneSecChangeRate, containsThreeSecChangeRate);
            }
        }

        public static bool ConvertFile(string inputPath, string outputPath, OutputType outputType, IDictionary<string, string> additionalPropertyValues, bool containsOneSecChangeRate = false, bool containsThreeSecChangeRate = false)
        {
            if (!string.Equals($".{outputType}", Path.GetExtension(outputPath), StringComparison.OrdinalIgnoreCase))
            {
                // output path is a directory, generate a file name based on the input file name and output type.
                var fileName = Path.GetFileNameWithoutExtension(inputPath) + "." + outputType.ToString().ToLowerInvariant();
                outputPath = Path.Combine(outputPath, fileName);
            }

            if (Path.GetDirectoryName(outputPath) is string dir && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            switch (outputType)
            {
                case OutputType.CSV:
                    var csvConverter = new FitToCsvConverter(SupportProperties.AllProperties, additionalPropertyValues);
                    return csvConverter.ConvertToCsvFile(inputPath, outputPath, containsOneSecChangeRate, containsThreeSecChangeRate);

                case OutputType.JSON:
                    // Implement JSON conversion logic here
                    throw new NotImplementedException("JSON conversion is not implemented yet.");

                default:
                    throw new ArgumentException($"Unsupported output type: {outputType}");
            }
        }
    }

    internal class Options
    {
        public string InputPath { get; set; } = string.Empty;
        public string OutputPath { get; set; } = string.Empty;
        public OutputType OutputType { get; set; } = OutputType.CSV;
        public string DirectoryNameFormat { get; set; } = string.Empty;
        public bool ContainsOneSecChangeRate { get; set; } = true;
        public bool ContainsThreeSecChangeRate { get; set; } = true;
    }

    internal enum OutputType
    {
        CSV,
        JSON
    }
}
