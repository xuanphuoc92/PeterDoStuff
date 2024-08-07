﻿using System.Dynamic;

namespace PeterDoStuff.MudWasmHosted.Shared
{
    public interface CryptographyApi
    {
        public Task<byte[]> HashArgon2id(Argon2idBody body);
        public Task<byte[]> HashArgon2idQuick(Argon2idBody body);
        
        public Task<byte[]> EncryptAes(AesBody body);
        public Task<byte[]> DecryptAes(AesBody body);
        
        public Task<RsaKeys> GenerateRsaKeys();
        public Task<byte[]> EncryptRsa(RsaBody body);
        public Task<byte[]> DecryptRsa(RsaBody body);
        public Task<byte[]> SignRsa(RsaBody body);
        public Task<bool> VerifyRsa(RsaBody body);
    }

    public record Argon2idBody(byte[] Input, byte[] Salt);
    public record AesBody(byte[] Input, byte[] Key, byte[] IV);
    public record RsaKeys(string Public, string Private);
    public record RsaBody(byte[] Input, string Key, byte[] Hash = null);
}
