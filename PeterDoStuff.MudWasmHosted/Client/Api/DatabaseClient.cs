using PeterDoStuff.MudWasmHosted.Shared;
using PeterDoStuff.Tools;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public class DatabaseClient : ApiClient, DatabaseApi
    {
        protected override string Route => "api/Database";

        private string _key;
        public DatabaseClient SetKey(string key)
        {
            _key = key;
            return this;
        }

        public Task<DbAccess> Access(string accessKey)
            => SendToApi<DbAccess>("Access", accessKey);

        public Task<QueryOutput> Post(string sql)
            => SendToApi<QueryOutput>("", sql, ("SimpleAuth", _key));
    }
}
