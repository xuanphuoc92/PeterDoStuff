using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Database;
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
        public Task<DbOutput> Post([FromBody] string sql)
        {
            return _db.ExecuteOrQueryAsync(sql);
        }

        private const string DEFAULT_ACCESS_KEY = "access";

        [HttpPost]
        [Route("access")]
        public DbAccess Access([FromBody] string accessKey)
        {
            var dbAccess = new DbAccess();
            
            var databaseAccessKey = Environment.GetEnvironmentVariable("DatabaseAccessKey");
            if (databaseAccessKey == null)
            {
                dbAccess.Warning = "You are using the default access key. Please set Environment Variable [DatabaseAccessKey] to stop using the default access key.";
                databaseAccessKey = DEFAULT_ACCESS_KEY;
            }

            dbAccess.IsSuccess = accessKey == databaseAccessKey;
            dbAccess.Token = dbAccess.IsSuccess ? databaseAccessKey : "";

            return dbAccess;
        }
    }
}
