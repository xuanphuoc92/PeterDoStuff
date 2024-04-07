using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.MudWasmHosted.Shared;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EnvironmentController : ControllerBase, EnvironmentApi
    {
        private const string empty = "Such an empty environment... Why not try setting an Environment Variable named MyFirstEnvironment?";

        [HttpGet]
        [Route("GetMyFirstEnvironment")]
        public async Task<string> GetMyFirstEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("MyFirstEnvironment");
            return environment == null
                ? empty
                : $"Huray! MyFirstEnvironment is set as {environment}";
        }

        [HttpGet]
        public async Task<string> Get(string key)
        {
            return Environment.GetEnvironmentVariable(key) ?? "";
        }
    }
}
