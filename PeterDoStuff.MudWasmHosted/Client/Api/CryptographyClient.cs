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

        public Task<RsaKeys> GenerateRsaKeys(RsaKeyConfig config)
            => SendToApi<RsaKeys>("GenerateRsaKeys", config);

        public Task<byte[]> EncryptRsa(RsaBody body)
            => SendToApi<byte[]>("EncryptRsa", body);

        public Task<byte[]> DecryptRsa(RsaBody body)
            => SendToApi<byte[]>("DecryptRsa", body);

        public Task<byte[]> SignRsa(RsaBody body)
            => SendToApi<byte[]>("SignRsa", body);

        public Task<bool> VerifyRsa(RsaBody body)
            => SendToApi<bool>("VerifyRsa", body);
    }
}
