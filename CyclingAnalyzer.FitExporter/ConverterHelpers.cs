using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyclingAnalyzer.FitExporter
{
    internal class ConverterHelpers
    {
        /// <summary>
        /// Get map values from a format string and input string. For example, if the format is "{Name}_{BirthYear}_{Height}_{Weight}", and the value is "QQ_1980_180_78", it will return a dictionary with keys "Name", "BirthYear", "Height", and "Weight" and their corresponding values.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetMapValues(string format, string input)
        {
            if (string.IsNullOrWhiteSpace(format))
            {
                return new Dictionary<string, string>();
            }

            List<string> keys = new List<string>();
            List<string> seperators = new List<string>();

            StringBuilder keyBuilder = null;
            StringBuilder separatorBuilder = new StringBuilder();
            separatorBuilder.Append("^");

            foreach (var c in format)
            {
                if (c == '{')
                {
                    if (separatorBuilder == null || separatorBuilder.Length == 0)
                    {
                        Console.WriteLine("Invalid format string: " + format);
                        return new Dictionary<string, string>();
                    }

                    seperators.Add(separatorBuilder.ToString());
                    separatorBuilder = null;
                    
                    keyBuilder = new StringBuilder();
                }
                else if (c == '}')
                {
                    if (keyBuilder == null || keyBuilder.Length == 0)
                    {
                        Console.WriteLine("Invalid format string: " + format);
                        return new Dictionary<string, string>();
                    }

                    keys.Add(keyBuilder.ToString());
                    keyBuilder = null;

                    separatorBuilder = new StringBuilder();
                }
                else
                {
                    if (keyBuilder != null)
                    {
                        keyBuilder.Append(c);
                    }
                    else if (separatorBuilder != null)
                    {
                        separatorBuilder.Append(c);
                    }
                    else
                    { 
                        Console.WriteLine("Invalid format string: " + format);
                        return new Dictionary<string, string>();
                    }
                }
            }

            if (separatorBuilder != null)
            {
                separatorBuilder.Append("$");
                seperators.Add(separatorBuilder.ToString());
            }
            
            var keyIndex = 0;
            var loopingStartIndex = 0;

            var result = new Dictionary<string, string>();

            foreach (var separator in seperators)
            {
                if (keyIndex >= keys.Count || loopingStartIndex > input.Length - 1)
                {
                    break;
                }

                if (separator.StartsWith("^"))
                {
                    loopingStartIndex += separator.Length - 1;
                    continue;
                }
                else if (separator.EndsWith("$"))
                {
                    // this is the last separator, we can use the rest of the string as the value.
                    var value = input.Substring(loopingStartIndex);
                    result.Add(keys[keyIndex], value);
                }
                else
                {
                    var foundIndex = input.IndexOf(separator, loopingStartIndex, StringComparison.OrdinalIgnoreCase);

                    if (foundIndex <= 0)
                    {
                        // value not match the format, insert empty value
                        result.Add(keys[keyIndex], string.Empty);
                    }
                    else
                    {
                        // found the value.
                        var value = input.Substring(loopingStartIndex, foundIndex - loopingStartIndex);
                        result.Add(keys[keyIndex], value);

                        loopingStartIndex = foundIndex + separator.Length;
                    }
                }

                keyIndex++;
            }

            return result;
        }
    }
}
