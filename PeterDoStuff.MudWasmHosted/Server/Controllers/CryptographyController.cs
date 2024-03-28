using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.MudWasmHosted.Shared;
using PeterDoStuff.Extensions;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CryptographyController : ControllerBase, CryptographyApi
    {
        [HttpPost]
        [Route("HashArgon2id")]
        public async Task<byte[]> HashArgon2id(Argon2idBody body)
        {
            return body.Input
                .HashArgon2id(body.Salt);
        }

        [HttpPost]
        [Route("HashArgon2idQuick")]
        public async Task<byte[]> HashArgon2idQuick([FromBody] Argon2idBody body)
        {
            return body.Input
                .HashArgon2id(body.Salt,
                    iterations: 2,
                    memorySize: 16,
                    degreeOfParallelism: 1,
                    hashLength: 16
                );
        }
    }
}
