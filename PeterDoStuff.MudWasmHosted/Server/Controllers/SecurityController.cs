using Microsoft.AspNetCore.Mvc;
using MudBlazor.Extensions;
using PeterDoStuff.Extensions;
using PeterDoStuff.MudWasmHosted.Shared;
using System.Dynamic;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{
    internal static class ApiExtensions
    {
        public static byte[] GetByteArray(this IDictionary<string, object> @this, string propertyName)
            => @this
            .GetPropertyValue(propertyName)
            .ToString()
            .ToByteArrayAsBase64String();

        public static string GetString(this IDictionary<string, object> @this, string propertyName)
            => @this
            .GetPropertyValue(propertyName)
            .ToString();
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        [HttpPost]
        [Route("HashArgon2id")]
        public byte[] HashArgon2id([FromBody] ExpandoObject body)
        {
            byte[] input = body.GetByteArray("input");
            byte[] salt = body.GetByteArray("salt");
            return input
                .HashArgon2id(salt);
        }

        [HttpPost]
        [Route("HashArgon2idQuick")]
        public byte[] HashArgon2idQuick([FromBody] ExpandoObject body)
        {
            byte[] input = body.GetByteArray("input");
            byte[] salt = body.GetByteArray("salt");
            return input
                .HashArgon2id(salt,
                    iterations: 1,
                    memorySize: 10,
                    degreeOfParallelism: 1
                );
        }

        [HttpPost]
        [Route("EncryptAes")]
        public byte[] EncryptAes([FromBody] ExpandoObject body)
        {
            byte[] input = body.GetByteArray("input");
            byte[] key = body.GetByteArray("key");
            byte[] iv = body.GetByteArray("iv");
            return input.EncryptAES(key, iv);
        }

        [HttpPost]
        [Route("DecryptAes")]
        public byte[] DecryptAes([FromBody] ExpandoObject body)
        {
            byte[] input = body.GetByteArray("input");
            byte[] key = body.GetByteArray("key");
            byte[] iv = body.GetByteArray("iv");
            return input.DecryptAES(key, iv);
        }

        [HttpPost]
        [Route("GenerateRsaKeys")]
        public RsaKeys GenerateRsaKeys()
        {
            var keys = SecurityExtensions.GenerateRSAKeys();
            return new RsaKeys(keys.Public, keys.Private);
        }

        [HttpPost]
        [Route("EncryptRsa")]
        public byte[] EncryptRsa([FromBody] ExpandoObject body)
        {
            byte[] input = body.GetByteArray("input");
            string publicKey = body.GetString("publicKey");
            return input                
                .EncryptRSA(publicKey);
        }

        [HttpPost]
        [Route("DecryptRsa")]
        public byte[] DecryptRsa([FromBody] ExpandoObject body)
        {
            byte[] input = body.GetByteArray("input");
            string privateKey = body.GetString("privateKey");
            return input
                .DecryptRSA(privateKey);
        }

        [HttpPost]
        [Route("SignRsa")]
        public byte[] SignRsa([FromBody] ExpandoObject body)
        {
            var input = body.GetByteArray("input");
            var privateKey = body.GetString("privateKey");
            return input
                .SignRSA(privateKey);
        }

        [HttpPost]
        [Route("VerifyRsa")]
        public bool VerifyRsa([FromBody] ExpandoObject body)
        {
            var input = body.GetByteArray("input");
            var hash = body.GetByteArray("hash");
            var publicKey = body.GetString("publicKey");
            return input                
                .VerifyRSA(hash, publicKey);
        }
    }
}
