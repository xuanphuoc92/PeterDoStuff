using PeterDoStuff.MudWasmHosted.Client.Extensions;
using PeterDoStuff.MudWasmHosted.Shared;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public class CryptographyClient : ApiClient, CryptographyApi
    {
        public Task<byte[]> HashArgon2idQuick(CryptographyApi.Argon2idBody body) 
            => SendToApi<byte[]>(HttpMethod.Post, "api/Cryptography/HashArgon2idQuick", body);
    }
}
