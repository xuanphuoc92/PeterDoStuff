using PeterDoStuff.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Tools
{
    public class QueryEditor
    {
        private BaseDb Db { get; set; }
        public QueryEditor(BaseDb db)
        {
            Db = db;
        }

        public async Task<QueryOutput> ExecuteOrQueryAsync(string sql, params object[] parameters)
        {
            // To acquire both outputs without causing double commits, make 2 transactions:
            // First one acquring the Query output and is then rolledback
            // Second one acquring the Execute output and is then committed

            IEnumerable<dynamic> queryOutput;
            using (var conn = Db.Open())
            {
                queryOutput = await conn.QueryAsync(sql, parameters);
                //conn.Commit();
            }

            int executeOutput;
            using (var conn = Db.Open())
            {
                executeOutput = await conn.ExecuteAsync(sql, parameters);
                conn.Commit();
            }

            return new QueryOutput()
            {
                Query = queryOutput.Cast<IDictionary<string, object>>(),
                Execute = executeOutput
            };
        }
    }

    public class QueryOutput
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
}
