using Dapper;
using System.Data;
using System.Data.Odbc;

namespace ODBC
{
    public static class Main
    {
        #region Key Configuration
        /// <summary>
        /// Sets the DSN to connect to.
        /// Always set this prior to calling any query functions.
        /// </summary>
        public static string DSN { get; set; }
        #endregion

        #region Generic
        /// <summary>
        /// Query raw DataTable
        /// </summary>
        public static DataTable Query(string query)
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            DataTable dataTable = new DataTable();
            dataTable.Load(new OdbcCommand(query, odbcConnection).ExecuteReader());
            odbcConnection.Close();
            return dataTable;
        }
        /// <summary>
        /// Select strongly typed rows from the query.
        /// This is one of THE most commonly used function of this library.
        /// </summary>
        public static TType[] Select<TType>(string query)
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");

            using var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            return odbcConnection.Query<TType>(query).ToArray();
        }
        /// <summary>
        /// Execute an insert query.
        /// Automatically commits.
        /// Equivalent to Command(); This function is just provided for semantic clarity.
        /// </summary>
        public static void Insert(string query)
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
        /// <summary>
        /// Execute an update query.
        /// Automatically commits.
        /// Equivalent to Command(); This function is just provided for semantic clarity.
        /// </summary>
        public static void Update(string query)
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
        /// <summary>
        /// Execute a delete query.
        /// Automatically commits.
        /// Equivalent to Command(); This function is just provided for semantic clarity.
        /// </summary>
        public static void Delete(string query)
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
        /// <summary>
        /// Execute arbitrary SQL command.
        /// Automatically commits.
        /// </summary>
        public static void Command(string query)
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");

            var odbcConnection = new OdbcConnection($"DSN={DSN}");
            odbcConnection.Open();
            new OdbcCommand(query, odbcConnection).ExecuteNonQuery();
            odbcConnection.Close();
        }
        #endregion

        #region Transactional
        /// <summary>
        /// Starts a transaction. The transaction object has the same sort of interfaces as the Main interface.
        /// </summary>
        public static Transaction OpenTransaction()
        {
            if (DSN == null)
                throw new ArgumentNullException("DSN is not set.");
            return new Transaction(DSN);
        }
        #endregion
    }
}