using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Database
{
    public class MemoryDb : IDisposable
    {
        private readonly SQLiteConnection _sharedConnection = new SQLiteConnection("Data Source=:memory:");
        public MemoryDb()
        {
            _sharedConnection.Open();
        }

        public void Dispose()
        {
            _sharedConnection.Close();
            _sharedConnection.Dispose();
        }

        public MemoryConnection Open()
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

    public class MemoryConnection : IDisposable
    {
        private readonly SQLiteConnection _sharedConnection;
        private readonly SQLiteTransaction _transaction;

        internal MemoryConnection(SQLiteConnection sharedConnection)
        {
            _sharedConnection = sharedConnection;
            _transaction = _sharedConnection.BeginTransaction();
        }

        /// <summary>
        /// Query by SQL with parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">E.g. "SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
        {
            var command = SqlCommand.New(sql, parameters);
            object param = new DynamicParameters(command.Parameters);
            return _sharedConnection.QueryAsync<T>(command.Sql, param);
        }

        /// <summary>
        /// Query by SQL with parameters
        /// </summary>
        /// <param name="sql">E.g. "SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, params object[] parameters)
        {
            return QueryAsync<dynamic>(sql, parameters);
        }

        /// <summary>
        /// Execute SQL commands with parameters
        /// </summary>
        /// <param name="sql">E.g. "Update [_TestTable] Set Amount = 0 where ID = {0} and CreatedTime > {1};</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public Task<int> ExecuteAsync(string sql, params object[] parameters) 
        {
            var command = SqlCommand.New(sql, parameters);
            object param = new DynamicParameters(command.Parameters);
            return _sharedConnection.ExecuteAsync(sql, param, _transaction);
        }

        /// <summary>
        /// Commit the transaction
        /// </summary>
        public void Commit()
        {
            _transaction.Commit();
        }
                
        public void Dispose()
        {
            _transaction.Dispose();
        }

        /// <summary>
        /// Query whether the table exists
        /// </summary>
        /// <param name="table">Name of the table</param>
        /// <returns></returns>
        public async Task<bool> TableExists(string table)
        {
            var queryResult = await QueryAsync(
                sql: "SELECT name FROM sqlite_master WHERE sqlite_master.type = 'table' AND name = {0}", 
                parameters: table);
            return queryResult.Any();
        }
    }
}
