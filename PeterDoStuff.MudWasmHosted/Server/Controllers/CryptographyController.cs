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

        [HttpPost]
        [Route("GenerateRsaKeys")]
        public async Task<RsaKeys> GenerateRsaKeys()
        {
            var keys = SecurityExtensions.GenerateRSAKeys();
            return new RsaKeys(keys.Public, keys.Private);
        }

        [HttpPost]
        [Route("EncryptRsa")]
        public async Task<byte[]> EncryptRsa(RsaBody body)
            => body.Input.EncryptRSA(body.Key);

        [HttpPost]
        [Route("DecryptRsa")]
        public async Task<byte[]> DecryptRsa(RsaBody body)
            => body.Input.DecryptRSA(body.Key);

        [HttpPost]
        [Route("SignRsa")]
        public async Task<byte[]> SignRsa(RsaBody body)
            => body.Input.SignRSA(body.Key);

        [HttpPost]
        [Route("VerifyRsa")]
        public async Task<bool> VerifyRsa(RsaBody body)
            => body.Input.VerifyRSA(body.Hash, body.Key);
    }
}
