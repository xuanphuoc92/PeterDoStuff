using System.Dynamic;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public interface CryptographyApi
    {
        public Task<byte[]> HashArgon2id(Argon2idBody body);
        public Task<byte[]> HashArgon2idQuick(Argon2idBody body);
        public Task<byte[]> EncryptAes(AesBody body);
        public Task<byte[]> DecryptAes(AesBody body);
    }

    public record Argon2idBody(byte[] Input, byte[] Salt);
    public record AesBody(byte[] Input, byte[] Key, byte[] IV);
}
