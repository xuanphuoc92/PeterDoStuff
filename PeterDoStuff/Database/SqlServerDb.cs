﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            _connection = new SqlConnection(connString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
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

        public override Task<bool> TableExists(string table)
        {
            throw new NotImplementedException();
        }
    }
}