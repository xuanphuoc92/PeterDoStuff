using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Database;

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
    }
}
