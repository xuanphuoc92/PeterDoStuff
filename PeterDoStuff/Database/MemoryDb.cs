﻿using Dapper;
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

        public override BaseConnection Open()
        {
            return new MemoryConnection(_sharedConnection);
        }

        public async Task<DbOutput> ExecuteOrQueryAsync(string sql, params object[] parameters)
        {
            // To acquire both outputs without causing double commits, make 2 transactions:
            // First one is rolledback and second one is committed

            IEnumerable<dynamic> queryOutput;
            using (var conn = Open())
            {
                queryOutput = await conn.QueryAsync(sql, parameters);
                //conn.Commit();
            }

            int executeOutput;
            using (var conn = Open())
            {
                executeOutput = await conn.ExecuteAsync(sql, parameters);
                conn.Commit();
            }

            return new DbOutput()
            {
                Query = queryOutput,
                Execute = executeOutput                
            };
        }
    }

    public class DbOutput
    {
        public IEnumerable<dynamic> Query { get; set; }
        public int Execute { get; set; }
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
        public override void Commit()
        {
            _transaction.Commit();
        }

        /// <summary>
        /// Dispose the transaction (if not committed, the transaction will be rolled back)
        /// </summary>
        public override void Dispose()
        {
            _transaction.Dispose();
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