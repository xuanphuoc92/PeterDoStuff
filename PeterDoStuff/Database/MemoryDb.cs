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
    }

    public class MemoryConnection : IDisposable
    {
        private readonly SQLiteConnection _sharedConnection;
        private readonly SQLiteTransaction _transaction;

        public MemoryConnection(SQLiteConnection sharedConnection)
        {
            _sharedConnection = sharedConnection;
            _transaction = _sharedConnection.BeginTransaction();
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            return _sharedConnection.QueryAsync<T>(sql, param, _transaction);
        }

        public Task<IEnumerable<dynamic>> QueryAsync(string sql, object? param = null)
            => QueryAsync<dynamic>(sql, param);

        public Task<IEnumerable<dynamic>> QueryAsync(Command command)
        {
            object param = new DynamicParameters(command.Parameters);
            return QueryAsync(command.Expression, param);
        }

        public Task<int> ExecuteAsync(string sql, object? param = null) 
        {
            return _sharedConnection.ExecuteAsync(sql, param, _transaction);
        }

        //public Task<int> ExecuteAsync(Command command)
        //{
        //    object param = new DynamicParameters(command.Parameters);
        //    return ExecuteAsync(command.Expression, param);
        //}

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public async Task<bool> TableExists(string table)
        {
            var command = Command.New("SELECT name FROM sqlite_master WHERE sqlite_master.type = 'table' AND name = {0}", table);
            var queryResult = await QueryAsync(command);
            return queryResult.Any();
        }
    }
}
