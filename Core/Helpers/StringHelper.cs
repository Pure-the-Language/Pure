namespace Core.Helpers
{
    public static class StringHelper
    {
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = (hash1 << 5) + hash1 ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = (hash2 << 5) + hash2 ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }
        public static string[] SplitArgumentsLikeCsv(this string line, char separator = ',', bool ignoreEmptyEntries = false)
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
