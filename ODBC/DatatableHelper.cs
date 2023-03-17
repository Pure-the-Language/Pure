using ConsoleTables;
using System.Data;
using System.Text;

namespace ODBC
{
    public static class DataTableHelper
    {
        #region Conversion
        public static string ToCSV(this DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                {
                    string value = field.ToString();
                    string escapeQuotes = value.Contains('"')
                        ? string.Concat("\"", value.Replace("\"", "\"\""), "\"")
                        : value;
                    string addQuotes = escapeQuotes.Contains(',') ? $"\"{escapeQuotes}\"" : escapeQuotes;
                    return addQuotes;
                });
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }
        public static string Print(this DataTable dataTable)
        {
            var consoleTable = new ConsoleTable();
            var columns = Enumerable.Range(0, dataTable.Columns.Count).Select(i => dataTable.Columns[i].ColumnName).ToArray();
            consoleTable.AddColumn(columns);

            foreach (DataRow dr in dataTable.Rows)
            {
                var items = dr.ItemArray.Select(i => i.ToString()).ToArray();
                consoleTable.AddRow(items);
            }
            consoleTable.Write();
            return consoleTable.ToString();
        }
        #endregion
    }
}
