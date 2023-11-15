using Dapper;
using System.Data;
using System.Data.Odbc;

namespace ODBC
{
    public class Transaction: IDisposable
    {
        #region Fields
        private readonly OdbcTransaction OdbcTransaction;
        private readonly OdbcConnection OdbcConnection;
        private string DSN;
        #endregion

        #region Lifetime Management
        public Transaction(string dsn)
        {
            DSN = dsn ?? throw new ArgumentNullException($"DSN is null");

            OdbcConnection = new OdbcConnection($"DSN={DSN}");
            OdbcConnection.Open();
            OdbcTransaction = OdbcConnection.BeginTransaction();
        }
        public void Close() => Dispose();
        public void Dispose()
        {
            OdbcTransaction.Commit();
            OdbcTransaction.Dispose();
            OdbcConnection.Close();
            OdbcConnection.Dispose();
        }
        #endregion

        #region Methods
        public DataTable Query(string query)
        {
            DataTable dataTable = new DataTable();
            dataTable.Load(new OdbcCommand(query, OdbcConnection, OdbcTransaction).ExecuteReader());
            return dataTable;
        }
        public TType[] Select<TType>(string query)
        {
            return OdbcConnection.Query<TType>(query, OdbcTransaction).ToArray();
        }
        public void Insert(string query)
        {
            new OdbcCommand(query, OdbcConnection, OdbcTransaction).ExecuteNonQuery();
        }
        public void Update(string query, Transaction transaction)
        {
            new OdbcCommand(query, OdbcConnection, OdbcTransaction).ExecuteNonQuery();
        }
        public void Delete(string query, Transaction transaction)
        {
            new OdbcCommand(query, OdbcConnection, OdbcTransaction).ExecuteNonQuery();
        }
        public void Command(string query)
        {
            new OdbcCommand(query, OdbcConnection, OdbcTransaction).ExecuteNonQuery();
        }
        #endregion
    }
}
