using Dapper;
using System.Data;
using System.Data.Odbc;

namespace ODBC
{
    public static class Main
    {
        public static string DSN { get; set; }
        public static DataTable Query(string query)
        {
            if (DSN == null)
            {
                Console.WriteLine("DSN is not set.");
                return null;
            }

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            DataTable dataTable = new DataTable();
            dataTable.Load(new OdbcCommand(query, odbcConnection).ExecuteReader());
            odbcConnection.Close();
            return dataTable;
        }
        public static TType[] Select<TType>(string query)
        {
            if (DSN == null)
            {
                Console.WriteLine("DSN is not set.");
                return null;
            }

            using var oracleConnection = new OdbcConnection($"DSN={DSN}");
            oracleConnection.Open();
            return oracleConnection.Query<TType>(query).ToArray();
        }
        public static void Insert(string query)
        {
            if (DSN == null)
            {
                Console.WriteLine("DSN is not set.");
                return;
            }

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
        public static void Update(string query)
        {
            if (DSN == null)
            {
                Console.WriteLine("DSN is not set.");
                return;
            }

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
        public static void Delete(string query)
        {
            if (DSN == null)
            {
                Console.WriteLine("DSN is not set.");
                return;
            }

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
    }
}