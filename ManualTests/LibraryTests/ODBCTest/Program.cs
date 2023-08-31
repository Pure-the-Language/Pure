namespace ODBCTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ODBC.Main.DSN = "SQLite3 Datasource";
            var results = ODBC.Main.Select<(string Name, double Value)>("select * from MyTable");
            Console.WriteLine();
        }
    }
}