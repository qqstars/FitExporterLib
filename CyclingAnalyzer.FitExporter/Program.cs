using CyclingAnalyzer.FitExporter;
using System.ComponentModel.Design;

class Program
{
    private static IDictionary<string, string[]> ArgsMapping = new Dictionary<string, string[]>
    {
        { "help", new string[] { "--help", "-h", "/h" } },
        { "input_path", new string[] { "--input", "-i", "/i" } },
        { "output_path", new string[] { "--output", "-o", "/o" } },
        { "output_type", new string[] { "--type", "-t", "/t" } },
        { "directory_name_format", new string[] { "--format", "-f", "/f" } },
        { "contains_one_sec_change_rate", new string[] { "--onesec", "-1", "/1" } },
        { "contains_three_sec_change_rate", new string[] { "--threesec", "-3", "/3" } }
    };

    static void Main(string[] args)
    {
        var argsDict = ReadArgs(args);

        if (argsDict.ContainsKey("help"))
        {
            ShowHelp();
            return;
        }

        Console.WriteLine("Cycling Analyzer Fit Exporter Parameters:");
        Console.WriteLine(string.Join("\r\n", argsDict.Select(kvp => $"{kvp.Key}={kvp.Value}")));
        Console.WriteLine("---------------------------------- Output ----------------------------------");
        Converter.Convert(GenerateOptions(argsDict));
    }

    private static void ShowHelp()
    { 
        Console.WriteLine("Cycling Analyzer Fit Exporter Help:");
        Console.WriteLine("Usage: CyclingAnalyzer.FitExporter [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --help, -h, /h                              Show this help message");
        Console.WriteLine("  --input=<path>, -i <path>, /i <path>        Specify the input path for the FIT file. If it is not set, ");
        Console.WriteLine("                                              the program will look for the FIT files in the current directory.");
        Console.WriteLine("                                              Will iterate all sub-folders under the path.");
        Console.WriteLine("  --output=<path>, -o <path>, /o <path>       Specify the output path for the exported data. If it is not set, ");
        Console.WriteLine("                                              the program will create a folder named 'Output' in the current directory.");
        Console.WriteLine("  --type=<type>, -t <type>, /t <type>         Specify the output type. Support types: CSV");
        Console.WriteLine("  --format=<format>, -f <format>, /f <format> Specify the directory name format.");
        Console.WriteLine("                                              Example: set as {Name}_{BirthYear}_{Height}_{Weight}_{Gender}");
        Console.WriteLine("                                              And the folder name QQ_1980_180_78_M:");
        Console.WriteLine("                                              will add additional columns into the output data.");
        Console.WriteLine("  --onesec=<true/false>, -1 <T/F>, /1 <T/F>   Specify if contains one second change rate.");
        Console.WriteLine("  --threesec=<true/false>, -3 <T/F>, /3 <T/F> Specify if contains three seconds change rate.");
    }

    private static IDictionary<string, string> ReadArgs(string[] args)
    {
        var result = new Dictionary<string, string>();
        for (var i = 0; i < args.Length; i++)
        {
            var argValue = args[i].Trim();
            string[] parts = new string[] { argValue.Trim() };
            if (argValue.StartsWith("--"))
            {
                parts = argValue.Split('=');
            }
            else if (argValue.StartsWith("-") || argValue.StartsWith("/"))
            {
                i++;
                parts = new string[] { argValue, i < args.Length ? args[i].Trim() : string.Empty };
            }
            else
            {
                continue;
            }

            var key = parts[0].Trim();

            var matchingKey = ArgsMapping.Where(kvp => kvp.Value.Any(v => string.Equals(v, key, StringComparison.OrdinalIgnoreCase))).Select(kvp => kvp.Key);
            if (matchingKey.Count() == 0)
            {
                continue;
            }

            key = matchingKey.First();

            if (parts.Length == 2)
            {
                result[key] = parts[1].Trim().Trim('"', '\'');
            }
            else
            {
                result[key] = string.Empty;
            }
        }
        return result;
    }

    private static Options GenerateOptions(IDictionary<string, string> options)
    {
        if (!Enum.TryParse<OutputType>(options.ContainsKey("output_type") ? options["output_type"] : string.Empty, true, out OutputType outputType))
        { 
            outputType = OutputType.CSV;
        }

        var result = new Options
        {
            InputPath = options.ContainsKey("input_path") ? options["input_path"] : Environment.CurrentDirectory,
            OutputPath = options.ContainsKey("output_path") ? options["output_path"] : Path.Combine(Environment.CurrentDirectory, "Output"),
            OutputType = outputType,
            DirectoryNameFormat = options.ContainsKey("directory_name_format") ? options["directory_name_format"] : string.Empty,
            ContainsOneSecChangeRate = options.ContainsKey("contains_one_sec_change_rate") && bool.TryParse(options["contains_one_sec_change_rate"], out bool containsOneSecChangeRate) ? containsOneSecChangeRate : true,
            ContainsThreeSecChangeRate = options.ContainsKey("contains_three_sec_change_rate") && bool.TryParse(options["contains_three_sec_change_rate"], out bool containsThreeSecChangeRate) ? containsThreeSecChangeRate : true,
        };

        return result;
    }
}
