using CsvHelper;
using System.Globalization;

namespace Data
{
    /// <summary>
    /// Entry point
    /// </summary>
    public static class Main
    {
        #region Reading
        /// <summary>
        /// Read typed csv records
        /// </summary>
        public static TType[] ReadCSV<TType>(string path, string delimiter = ",", bool containsHeader = true)
        {
            // TODO: delimiter and containsHeader not implemented
            using StreamReader reader = new(path);
            using CsvReader csv = new(reader, CultureInfo.InvariantCulture);
            IEnumerable<TType> records = csv.GetRecords<TType>();
            return records.ToArray();
        }
        #endregion

        #region Writing
        /// <summary>
        /// Save csv records to file
        /// </summary>
        public static void SaveCSV<TType>(string path, IEnumerable<TType> values, bool makeHeader = true)
        {
            // TODO: makeHeader not implemented
            using StreamWriter writer = new(path);
            using CsvWriter csv = new(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(values);
        }
        #endregion
    }
}
