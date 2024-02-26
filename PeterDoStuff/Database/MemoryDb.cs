using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Database
{
    public class MemoryDb : BaseDb
    {
        private readonly SQLiteConnection _sharedConnection = new SQLiteConnection("Data Source=:memory:");
        public MemoryDb()
        {
            _sharedConnection.Open();
        }

        public override void Dispose()
        {
            _sharedConnection.Close();
            _sharedConnection.Dispose();
        }

        protected override BaseConnection NewConnection()
        {
            return new MemoryConnection(_sharedConnection);
        }
    }

    public class MemoryConnection : BaseConnection
    {
        internal MemoryConnection(SQLiteConnection sharedConnection)
        {
            _connection = sharedConnection;
            _transaction = _connection.BeginTransaction();
        }

        /// <summary>
        /// Commit the transaction
        /// </summary>
        protected override void ActualCommit()
        {
            _transaction.Commit();
        }

        /// <summary>
        /// Dispose the transaction (if not committed, the transaction will be rolled back)
        /// </summary>
        protected override void ActualDispose()
        {
            _transaction.Dispose();
        }

        internal override async Task<bool> TableExists(string table)
        {
            var queryResult = await QueryAsync(
                sql: "SELECT name FROM sqlite_master WHERE sqlite_master.type = 'table' AND name = {0}", 
                parameters: table);
            return queryResult.Any();
        }
    }
}
