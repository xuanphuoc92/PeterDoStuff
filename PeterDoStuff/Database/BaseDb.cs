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
        public abstract void Dispose();

        protected abstract BaseConnection NewConnection();

        private int Scope { get; set; } = -1;
        internal bool IsOutermostScope() => Scope == 0;
        private void MoveDownScope() => Scope++;
        internal void MoveUpScope() => Scope--;

        internal BaseConnection? CurrentConn { get; set; }
        public BaseConnection Open()
        {
            MoveDownScope();
            if (IsOutermostScope()) // Create a new Connection for the Outermost Scope
            {
                CurrentConn = NewConnection();
                CurrentConn.Db = this;
            }
            CurrentConn.Register();
            return CurrentConn;
        }
    }


    public abstract class BaseConnection : IDisposable
    {
        protected DbConnection _connection;
        protected DbTransaction _transaction;

        protected abstract void OuterCommit();

        protected abstract void OuterDispose();

        internal BaseDb Db { get; set; }

        private List<bool> ConnCommits { get; set; } = new List<bool>();
        private Stack<int> ConnStack { get; set; } = new Stack<int>();
        private int CurrentConnIndex { get; set; }

        /// <summary>
        /// Register the connection, to track any inner rollback.
        /// </summary>
        internal void Register()
        {
            CurrentConnIndex = ConnCommits.Count;
            ConnCommits.Add(false);
            ConnStack.Push(CurrentConnIndex);
        }

        /// <summary>
        /// Commit the transaction
        /// </summary>
        public void Commit()
        {
            // Flag that the connection has been commited
            ConnCommits[CurrentConnIndex] = true;

            if (Db.IsOutermostScope())
            {
                if (ConnCommits.Contains(false))
                    throw new Exception("Cannot commit connection with non-committed inner connection(s)");
                OuterCommit();
            }
        }

        /// <summary>
        /// Dispose the transaction (if not committed, the transaction will be rolled back)
        /// </summary>
        public void Dispose()
        {
            // Pop the Current Connection
            // Get the last connection from the top of the stack and set it as the Current Connection
            ConnStack.Pop();
            CurrentConnIndex = ConnStack.Any() ? ConnStack.Peek() : -1;

            if (Db.IsOutermostScope())
            {
                OuterDispose();
                Db.CurrentConn = null;
                ConnCommits.Clear();
                ConnStack.Clear();
            }
            Db.MoveUpScope();
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
        public async Task<int> ExecuteAsync(string sql, params object[] parameters)
        {
            var command = SqlCommand.New(sql, parameters);
            object param = new DynamicParameters(command.Parameters);
            int executed = await _connection.ExecuteAsync(sql, param, _transaction);            
            return executed;
        }

        /// <summary>
        /// Query whether the table exists
        /// </summary>
        /// <param name="table">Name of the table</param>
        /// <returns></returns>
        public abstract Task<bool> TableExists(string table);
    }
}
