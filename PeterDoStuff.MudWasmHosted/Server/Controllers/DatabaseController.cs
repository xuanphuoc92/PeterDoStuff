using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Database;
using PeterDoStuff.MudWasmHosted.Server.Auth;
using PeterDoStuff.MudWasmHosted.Shared;
using PeterDoStuff.Tools;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase, DatabaseApi
    {
        //private readonly ILogger<DatabaseController> _logger;

        //public DatabaseController(ILogger<DatabaseController> logger)
        //{
        //    _logger = logger;
        //}

        private MemoryDb _db;
        public DatabaseController(MemoryDb db)
        {
            _db = db;
        }

        [HttpPost]
        [SimpleAuth(ENVIRONMENT_VAR_KEY, DEFAULT_ACCESS_KEY)]
        public Task<QueryOutput> Post([FromBody] string sql)
        {
            var queryEditor = new QueryEditor(_db);
            return queryEditor.ExecuteOrQueryAsync(sql);
        }

        private const string ENVIRONMENT_VAR_KEY = "DatabaseAccessKey";
        private const string DEFAULT_ACCESS_KEY = "access";

        [HttpPost]
        [Route("Access")]
        public async Task<DbAccess> Access([FromBody] string accessKey)
        {
            var databaseAccessKey = Environment.GetEnvironmentVariable(ENVIRONMENT_VAR_KEY)
                ?? DEFAULT_ACCESS_KEY;

            string warning = "";
            if (databaseAccessKey == DEFAULT_ACCESS_KEY)
                warning = "You are using the default access key. Please set Environment Variable [DatabaseAccessKey] to stop using the default access key.";

            bool isSuccess = accessKey == databaseAccessKey;
            return new DbAccess(isSuccess, warning, isSuccess ? databaseAccessKey : "");
        }
    }
}
