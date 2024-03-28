using PeterDoStuff.MudWasmHosted.Client.Extensions;
using PeterDoStuff.MudWasmHosted.Shared;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public class CryptographyClient : ApiClient, CryptographyApi
    {
        protected override string Route => "api/Cryptography";

        public Task<byte[]> HashArgon2idQuick(CryptographyApi.Argon2idBody body) 
            => SendToApi<byte[]>("HashArgon2idQuick", body);
    }
}
