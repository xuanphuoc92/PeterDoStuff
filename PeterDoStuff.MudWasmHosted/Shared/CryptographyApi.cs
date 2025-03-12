using System.Dynamic;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public interface CryptographyApi
    {
        public Task<byte[]> HashArgon2id(Argon2idBody body);
        public Task<byte[]> HashArgon2idQuick(Argon2idBody body);
        
        public Task<byte[]> EncryptAes(AesBody body);
        public Task<byte[]> DecryptAes(AesBody body);
        
        public Task<RsaKeys> GenerateRsaKeys(RsaKeyConfig config);
        public Task<byte[]> EncryptRsa(RsaBody body);
        public Task<byte[]> DecryptRsa(RsaBody body);
        public Task<byte[]> SignRsa(RsaBody body);
        public Task<bool> VerifyRsa(RsaBody body);
    }

    public record Argon2idBody(byte[] Input, byte[] Salt);
    public record AesBody(byte[] Input, byte[] Key, byte[] IV);

    public record RsaKeyConfig(RsaKeysFormat Format);
    public enum RsaKeysFormat
    {
        Spki_Pkcs8,
        Pem
    }
    public record RsaKeys(string Public, string Private);

    public record RsaBody(byte[] Input, string Key,
        byte[] Hash = null,
        RsaEncryptPadding EncryptPadding = RsaEncryptPadding.Oaep256,
        RsaSignHashing SignHashing = RsaSignHashing.Sha256);

    public enum RsaEncryptPadding
    {
        Oaep256,
        Pkcs1
    }

    public enum RsaSignHashing
    {
        Sha256,
        Sha512
    }
}
