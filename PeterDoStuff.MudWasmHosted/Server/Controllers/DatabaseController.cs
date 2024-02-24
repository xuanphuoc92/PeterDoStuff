using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Database;
using PeterDoStuff.MudWasmHosted.Server.Auth;
using PeterDoStuff.MudWasmHosted.Shared;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DatabaseController : ControllerBase
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
        public Task<DbOutput> Post([FromBody] string sql)
        {
            return _db.ExecuteOrQueryAsync(sql);
        }

        private const string ENVIRONMENT_VAR_KEY = "DatabaseAccessKey";
        private const string DEFAULT_ACCESS_KEY = "access";

        [HttpPost]
        [Route("access")]
        public DbAccess Access([FromBody] string accessKey)
        {
            var dbAccess = new DbAccess();
            
            var databaseAccessKey = Environment.GetEnvironmentVariable(ENVIRONMENT_VAR_KEY) 
                ?? DEFAULT_ACCESS_KEY;

            if (databaseAccessKey == DEFAULT_ACCESS_KEY)
                dbAccess.Warning = "You are using the default access key. Please set Environment Variable [DatabaseAccessKey] to stop using the default access key.";

            if (accessKey == databaseAccessKey)
            {
                dbAccess.IsSuccess = true;
                dbAccess.Token = dbAccess.IsSuccess ? databaseAccessKey : "";
            }
            return dbAccess;
        }
    }
}
