using Microsoft.AspNetCore.Mvc;
using PeterDoStuff.MudWasmHosted.Shared;
using PeterDoStuff.Extensions;
using System.Security.Cryptography;

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
        public async Task<RsaKeys> GenerateRsaKeys(RsaKeyConfig config)
        {
            var keys = SecurityExtensions.GenerateRSAKeysFull();
            if (config.Format == RsaKeysFormat.Spki_Pkcs8)
                return new RsaKeys(keys.PublicSpki, keys.PrivatePkcs8);
            return new RsaKeys(keys.PublicPem, keys.PrivatePem);
        }

        [HttpPost]
        [Route("EncryptRsa")]
        public async Task<byte[]> EncryptRsa(RsaBody body)
            => body.Input.EncryptRSA(body.Key, GetPadding(body.EncryptPadding));

        [HttpPost]
        [Route("DecryptRsa")]
        public async Task<byte[]> DecryptRsa(RsaBody body)
            => body.Input.DecryptRSA(body.Key, GetPadding(body.EncryptPadding));

        private static RSAEncryptionPadding GetPadding(RsaEncryptPadding? padding)
            => padding switch
            {
                RsaEncryptPadding.Oaep256 => RSAEncryptionPadding.OaepSHA256,
                RsaEncryptPadding.Pkcs1 => RSAEncryptionPadding.Pkcs1,
                _ => throw new Exception($"Unknown Padding: {padding?.ToString() ?? "NULL"}")
            };

        [HttpPost]
        [Route("SignRsa")]
        public async Task<byte[]> SignRsa(RsaBody body)
            => body.Input.SignRSA(body.Key, GetHashAlgo(body.SignHashing));

        [HttpPost]
        [Route("VerifyRsa")]
        public async Task<bool> VerifyRsa(RsaBody body)
            => body.Input.VerifyRSA(body.Hash, body.Key, GetHashAlgo(body.SignHashing));

        private static HashAlgorithmName GetHashAlgo(RsaSignHashing? hashing)
            => hashing switch
            {
                RsaSignHashing.Sha256 => HashAlgorithmName.SHA256,
                RsaSignHashing.Sha512 => HashAlgorithmName.SHA512,
                _ => throw new Exception($"Unknown Hashing: {hashing?.ToString() ?? "NULL"}")
            };
    }
}
