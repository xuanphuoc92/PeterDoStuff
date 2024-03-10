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
    /// <summary>
    /// Inteface to a virtual SQL database within the Memory, using SQLite.
    /// </summary>
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

    /// <summary>
    /// Inteface to a virtual SQL database's Connection/Transaction.
    /// </summary>
    public class MemoryConnection : BaseConnection
    {
        internal MemoryConnection(SQLiteConnection sharedConnection)
        {
            DbConnection = sharedConnection;
            DbTransaction = DbConnection.BeginTransaction();
        }

        protected override void CommitImplemenation()
        {
            DbTransaction.Commit();
        }

        protected override void DisposeImplemenation()
        {
            DbTransaction.Dispose();
        }

        public override async Task<bool> TableExists(string table)
        {
            var queryResult = await QueryAsync(
                sql: "SELECT name FROM sqlite_master WHERE sqlite_master.type = 'table' AND name = {0}", 
                parameters: table);
            return queryResult.Any();
        }
    }
}
