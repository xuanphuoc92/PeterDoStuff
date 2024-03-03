using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.Extensions;

namespace PeterDoStuff.MudWasmHosted.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
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
