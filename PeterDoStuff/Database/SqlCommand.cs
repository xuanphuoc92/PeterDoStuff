﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PeterDoStuff.Database
{
    public class SqlCommand
    {
        public const string SAMPLE_TEST_SQL = @"DROP TABLE IF EXISTS [_TestTable_];
CREATE TABLE [_TestTable_] ([Number] int, [Name] nvarchar(100));
INSERT INTO [_TestTable_] ([Number], [Name]) VALUES (1, 'Alice'), (2, 'Bob'), (3, 'Carol');
SELECT * FROM [_TestTable_];";

        public string Sql { get; private set; } = "";
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();

        private SqlCommand() { }
        
        public static SqlCommand New() => new SqlCommand();

        public static SqlCommand New(string sql, params object[] parameters)
        {
            var command = New();
            command.AppendLine(sql, parameters);
            return command; 
        }

        public SqlCommand AppendLine(string sql, object[] parameters)
        {
            Sql += RecordAndFormat(sql, parameters);
            return this;
        }

        private string RecordAndFormat(string sql, object[] parameters)
        {
            var keys = RecordParameters(parameters);
            return string.Format(sql, keys);
        }

        private string[] RecordParameters(object[] parameters)
        {
            List<string> keys = new List<string>();
            foreach (object parameter in parameters)
            {
                //// For SQL Server, the null parameter does not work.
                //// Therefore, it is better to hard-code null parameter value into the SQL script.
                //if (parameter == null)
                //{
                //    keys.Add("null");
                //    continue;
                //}

                var key = "@p" + Parameters.Count;
                Parameters.Add(key, parameter);
                keys.Add(key);
            }

            return keys.ToArray();
        }
    }
}