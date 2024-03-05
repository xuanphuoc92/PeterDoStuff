using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Extensions;
using PeterDoStuff.MudWasmHosted.Shared;

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

        [HttpGet]
        [Route("GenerateRsaKeys")]
        public RsaKeys GenerateRsaKeys()
        {
            var keys = SecurityExtensions.GenerateRSAKeys();
            return new RsaKeys(keys.Public, keys.Private);
        }

        [HttpPost]
        [Route("EncryptRsa")]
        public byte[] EncryptRsa(string input, [FromBody] string publicKey)
        {
            return input
                .ToByteArray()
                .EncryptRSA(publicKey);
        }

        [HttpPost]
        [Route("DecryptRsa")]
        public string DecryptRsa(string input, [FromBody] string privateKey)
        {
            return input
                .ToByteArrayAsHexString()
                .DecryptRSA(privateKey)
                .ToUTF8String();
        }

        [HttpPost]
        [Route("SignRsa")]
        public byte[] SignRsa(string input, [FromBody] string privateKey)
        {
            return input
                .ToByteArray()
                .SignRSA(privateKey);
        }

        [HttpPost]
        [Route("VerifyRsa")]
        public bool VerifyRsa(string input, string hash, [FromBody] string publicKey)
        {
            return input
                .ToByteArray()
                .VerifyRSA(hash.ToByteArrayAsHexString(), publicKey);
        }
    }
}
