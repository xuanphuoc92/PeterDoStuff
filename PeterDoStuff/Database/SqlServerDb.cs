using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Database
{
    public class SqlServerDb : BaseDb
    {
        private readonly string _connString;

        /// <summary>
        /// Example connection string: 
        /// "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
        /// </summary>
        /// <param name="connString"></param>
        public SqlServerDb(string connString)
        {
            _connString = connString;
        }

        public override BaseConnection Open()
        {
            return new SqlServerConnection(_connString);
        }

        public override void Dispose()
        {
        }
    }

    public class SqlServerConnection : BaseConnection
    {
        public SqlServerConnection(string connString)
        {
            try
            {
                _connection = new SqlConnection(connString);
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
            catch (Exception ex)
            {
                var message = new StringBuilder();
                message.AppendLine("Unable to connect to database.");
                message.AppendLine("Please check on the connection string.");
                message.AppendLine("More details: " + ex.Message);
                throw new Exception(message.ToString());
            }
        }

        public override void Commit()
        {
            _transaction.Commit();
            _connection.Close();
        }

        public override void Dispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
        }

        public override async Task<bool> TableExists(string table)
        {
            var queryResult = await QueryAsync(
                sql: "SELECT t.name FROM sys.tables t WHERE t.is_ms_shipped = 0 AND t.name = {0}",
                parameters: table);
            return queryResult.Any();
        }
    }
}
