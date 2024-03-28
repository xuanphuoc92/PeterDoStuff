using PeterDoStuff.MudWasmHosted.Client.Extensions;
using PeterDoStuff.MudWasmHosted.Shared;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public class CryptographyClient : ApiClient, CryptographyApi
    {
        protected override string Route => "api/Cryptography";        

        public Task<byte[]> HashArgon2id(Argon2idBody body)
            => SendToApi<byte[]>("HashArgon2id", body);

        public Task<byte[]> HashArgon2idQuick(Argon2idBody body) 
            => SendToApi<byte[]>("HashArgon2idQuick", body);

        public Task<byte[]> EncryptAes(AesBody body)
            => SendToApi<byte[]>("EncryptAes", body);

        public Task<byte[]> DecryptAes(AesBody body)
            => SendToApi<byte[]>("DecryptAes", body);
    }
}
