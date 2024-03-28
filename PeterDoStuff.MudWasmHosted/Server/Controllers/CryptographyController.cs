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
            => body.Input
                .HashArgon2id(body.Salt);

        [HttpPost]
        [Route("HashArgon2idQuick")]
        public async Task<byte[]> HashArgon2idQuick(Argon2idBody body) 
            => body.Input
                .HashArgon2id(body.Salt,
                    iterations: 2,
                    memorySize: 16,
                    degreeOfParallelism: 1,
                    hashLength: 16
                );


        [HttpPost]
        [Route("EncryptAes")]
        public async Task<byte[]> EncryptAes(AesBody body) 
            => body.Input.EncryptAES(body.Key, body.IV);

        [HttpPost]
        [Route("DecryptAes")]
        public async Task<byte[]> DecryptAes(AesBody body)
            => body.Input.DecryptAES(body.Key, body.IV);
    }
}
