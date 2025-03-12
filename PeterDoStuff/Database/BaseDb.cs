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
    /// <summary>
    /// Interface to an SQL Database (with support of nested/flat transactions).
    /// </summary>
    public abstract class BaseDb : IDisposable
    {
        public abstract void Dispose();

        protected abstract BaseConnection NewConnection();

        internal BaseConnection? MasterConnection { get; set; }
        internal void RemoveMasterConnection() => MasterConnection = null;

        /// <summary>
        /// Open a new connection/transaction with the database
        /// </summary>
        /// <returns></returns>
        public BaseConnection Open()
        {
            if (MasterConnection == null)
            {
                MasterConnection = NewConnection();
                MasterConnection.Db = this;
                return MasterConnection;
            }

            var nestedConnection = new NestedConnection(MasterConnection);
            nestedConnection.Db = this;
            return nestedConnection;
        }
    }

    /// <summary>
    /// Interface of wrapper for an SQL Database's Connection/Transaction.
    /// </summary>
    public abstract class BaseConnection : IDisposable
    {
        protected DbConnection DbConnection;
        protected DbTransaction DbTransaction;

        internal BaseDb Db { get; set; }

        private bool IsCommitted { get; set; } = false;
        internal bool ContainsRollback { get; set; } = false;

        protected abstract void CommitImplemenation();
        protected abstract void DisposeImplemenation();

        /// <summary>
        /// Commit the connection/transaction
        /// </summary>
        public void Commit()
        {
            if (ContainsRollback)
                throw new Exception("Contains Rollback in Nested Connections");
            CommitImplemenation();
            IsCommitted = true;
        }

        /// <summary>
        /// Dispose the connection/transaction (if not committed, the connection/transaction will be rolled back)
        /// </summary>
        public void Dispose()
        {
            if (IsCommitted == false)
                Db.MasterConnection.ContainsRollback = true;
            DisposeImplemenation();
            if (this is NestedConnection == false)
                Db.RemoveMasterConnection();
        }

        /// <summary>
        /// Query by SQL with parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">E.g. "SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};"</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> QueryAsync<T>(string sql, params object[] parameters)
        {
            var command = SqlCommand.New().Append(sql, parameters);
            return QueryAsync<T>(command);
        }

        /// <summary>
        /// Query by SqlCommand
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">SqlCommand.New("SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};", "1001a", DateTime.Today)</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<T>> QueryAsync<T>(SqlCommand command)
        {
            object param = new DynamicParameters(command.Parameters);
            return DbConnection.QueryAsync<T>(command.Sql, param, DbTransaction);
        }

        /// <summary>
        /// Query by SQL with parameters
        /// </summary>
        /// <param name="sql">E.g. "SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};"</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public Task<IEnumerable<dynamic>> QueryAsync(string sql, params object[] parameters)
        {
            return QueryAsync<dynamic>(sql, parameters);
        }

        /// <summary>
        /// Execute SQL commands with parameters
        /// </summary>
        /// <param name="sql">E.g. "Update [_TestTable] Set Amount = 0 where ID = {0} and CreatedTime > {1};"</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync(string sql, params object[] parameters)
        {
            var command = SqlCommand.New().Append(sql, parameters);
            return await ExecuteAsync(command);
        }

        /// <summary>
        /// Execute by SQLCommand
        /// </summary>
        /// <param name="command">SqlCommand.New("Update [_TestTable] Set Amount = 0 where ID = {0} and CreatedTime > {1};", "1001a", DateTime.Today)</param>
        /// <returns></returns>
        public virtual async Task<int> ExecuteAsync(SqlCommand command)
        {
            object param = new DynamicParameters(command.Parameters);
            int executed = await DbConnection.ExecuteAsync(command.Sql, param, DbTransaction);
            return executed;
        }

        /// <summary>
        /// Query whether the table exists.
        /// Q: Why is this abstract at the BaseDb?
        /// A: Different SQL databases will have different SQL commands to query whether a table exists.
        /// </summary>
        /// <param name="table">Name of the table</param>
        /// <returns></returns>
        public abstract Task<bool> TableExists(string table);
    }

    internal class NestedConnection : BaseConnection
    {
        private BaseConnection MasterConnection { get; set; }

        public NestedConnection(BaseConnection masterConnection)
        {
            MasterConnection = masterConnection;
        }

        public override Task<bool> TableExists(string table)
        {
            return MasterConnection.TableExists(table);
        }

        public override Task<IEnumerable<T>> QueryAsync<T>(SqlCommand command)
        {
            return MasterConnection.QueryAsync<T>(command);
        }

        public override Task<int> ExecuteAsync(SqlCommand command)
        {
            return MasterConnection.ExecuteAsync(command);
        }

        protected override void CommitImplemenation()
        {
            // Do nothing for commit of nested transaction
        }

        protected override void DisposeImplemenation()
        {
            // Do nothing for dispose of nested transaction
        }
    }
}
