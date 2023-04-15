using System.Text.RegularExpressions;

namespace Python
{
    internal class RuntimeHelper
    {
        internal static string FindPythonDLL()
        {
            foreach (string path in SplitArgumentsLikeCsv(Environment.GetEnvironmentVariable("PATH"), ';', true))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(path))
                    {
                        string filename = Path.GetFileName(file);
                        if (Regex.IsMatch(filename, @"python\d+\.dll"))
                            return file;
                    }
                }
                // Remark-cz: Certain paths might NOT be enumerable due to access issues
                catch (Exception) { continue; }
            }
            return null;

            static string[] SplitArgumentsLikeCsv(string line, char separator = ',', bool ignoreEmptyEntries = false)
            {
                // Remark-cz: We use Csv library mainly to allow automatic handling of double quotes
                string[] arguments = Csv.CsvReader.ReadFromText(line, new Csv.CsvOptions()
                {
                    HeaderMode = Csv.HeaderMode.HeaderAbsent,
                    Separator = separator
                }).First().Values;

                if (ignoreEmptyEntries)
                    return arguments.Where(a => !string.IsNullOrWhiteSpace(a)).ToArray();
                else
                    return arguments;
            }
        }
    }
}
