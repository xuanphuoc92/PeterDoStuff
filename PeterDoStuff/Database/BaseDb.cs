using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Database
{
    public abstract class BaseDb : IDisposable
    {
        public abstract BaseConnection Open();
        public abstract void Dispose();

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
                Query = queryOutput.Cast<IDictionary<string, object>>(),
                Execute = executeOutput
            };
        }
    }

    public class DbOutput
    {
        private IEnumerable<dynamic> _dynamicQuery = null;
        public IEnumerable<dynamic> DynamicQuery()
        {
            if (_dynamicQuery == null)
                _dynamicQuery = Query;
            return _dynamicQuery;
        }

        public IEnumerable<IDictionary<string, object>> Query { get; set; }
        public int Execute { get; set; }
    }

    public abstract class BaseConnection : IDisposable
    {
        protected DbConnection _connection;
        protected DbTransaction _transaction;

        /// <summary>
        /// Commit the transaction
        /// </summary>
        public abstract void Commit();

        /// <summary>
        /// Dispose the transaction (if not committed, the transaction will be rolled back)
        /// </summary>
        public abstract void Dispose();

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
            return _connection.QueryAsync<T>(command.Sql, param, _transaction);
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
            return _connection.ExecuteAsync(sql, param, _transaction);
        }

        /// <summary>
        /// Query whether the table exists
        /// </summary>
        /// <param name="table">Name of the table</param>
        /// <returns></returns>
        public abstract Task<bool> TableExists(string table);
    }
}
