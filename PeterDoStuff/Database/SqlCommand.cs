using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace PeterDoStuff.Database
{
    public static class SqlCommandExtension
    {
        public static IEnumerable<T> Query<T>(this DbConnection @this, SqlCommand command)
        {
            return @this.Query<T>(command.Sql, new DynamicParameters(command.Parameters));
        }

        public static int Execute(this DbConnection @this, SqlCommand command)
        {
            return @this.Execute(command.Sql, new DynamicParameters(command.Parameters));
        }
    }

    /// <summary>
    /// Instance of SQL Command with support to prevent SQL injection.
    /// </summary>
    public class SqlCommand
    {
        public const string SAMPLE_TEST_SQL = @"DROP TABLE IF EXISTS [_TestTable_];
CREATE TABLE [_TestTable_] ([Number] int, [Name] nvarchar(100));
INSERT INTO [_TestTable_] ([Number], [Name]) VALUES (1, 'Alice'), (2, 'Bob'), (3, 'Carol');
SELECT * FROM [_TestTable_];";

        public string Sql { get; private set; } = "";
        public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();

        private SqlCommand() { }
        
        /// <summary>
        /// Get a new empty SQL Command. Use Append() and AppendLine() method to add sql to the command.
        /// </summary>
        /// <returns></returns>
        public static SqlCommand New() => new SqlCommand();

        /// <summary>
        /// Append a line of sql to the existing command.
        /// </summary>
        /// <param name="sql">E.g. "SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};"</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public SqlCommand AppendLine(string sql, params object[] parameters)
        {
            Sql += RecordAndFormat(sql, parameters) + Environment.NewLine;
            return this;
        }

        /// <summary>
        /// Append sql to the existing command.
        /// </summary>
        /// <param name="sql">E.g. "SELECT * FROM [_TestTable] where ID = {0} and CreatedTime > {1};"</param>
        /// <param name="parameters">E.g. "1001a", DateTime.Today</param>
        /// <returns></returns>
        public SqlCommand Append(string sql, params object[] parameters)
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
