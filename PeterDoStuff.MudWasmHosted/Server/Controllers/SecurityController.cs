using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Extensions;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        [HttpGet]
        [Route("HashArgon2id")]
        public byte[] HashArgon2id(string input, string salt)
        {
            return input
                .ToByteArray()
                .HashArgon2id(salt.ToByteArrayAsHexString());
        }

        [HttpGet]
        [Route("HashArgon2idQuick")]
        public byte[] HashArgon2idQuick(string input, string salt)
        {
            return input
                .ToByteArray()
                .HashArgon2id(salt.ToByteArrayAsHexString(),
                    iterations: 1,
                    memorySize: 10,
                    degreeOfParallelism: 1
                );
        }

        [HttpGet]
        [Route("EncryptAes")]
        public byte[] EncryptAes(string input, string key, string iv)
        {
            return input.EncryptAES(key.ToByteArrayAsHexString(), iv.ToByteArrayAsHexString());
        }

        [HttpGet]
        [Route("DecryptAes")]
        public string DecryptAes(string input, string key, string iv)
        {
            return input.ToByteArrayAsHexString().DecryptAES(key.ToByteArrayAsHexString(), iv.ToByteArrayAsHexString());
        }
    }
}
