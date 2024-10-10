using Konscious.Security.Cryptography;
using System.Security.Cryptography;

namespace PeterDoStuff.Extensions
{
    /// <summary>
    /// Extensions for Security features such as Hashing and Encryption
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Compute the SHA256 Hash Value of a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] HashSHA256(this byte[] input)
        {
            using var hasher = SHA256.Create();
            return hasher.ComputeHash(input);
        }

        /// <summary>
        /// Compute the SHA512 Hash Value of a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] HashSHA512(this byte[] input)
        {
            using var hasher = SHA512.Create();
            return hasher.ComputeHash(input);
        }

        /// <summary>
        /// Compute the HMAC-SHA256 Hash Value of a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static byte[] HashHMACSHA256(this byte[] input, byte[] keys)
        {
            using var hasher = new HMACSHA256(keys);
            return hasher.ComputeHash(input);
        }

        /// <summary>
        /// Compute the HMAC-SHA512 Hash Value of a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static byte[] HashHMACSHA512(this byte[] input, byte[] keys)
        {
            using var hasher = new HMACSHA512(keys);
            return hasher.ComputeHash(input);
        }

        /// <summary>
        /// Generate a random AES key
        /// </summary>
        /// <param name="keySize">Size of keys in bits. Default 256. Valid values: 128, 192, 256.</param>
        /// <returns></returns>
        public static byte[] GenerateAesKey(int keySize = 256)
        {
            using var aes = Aes.Create();
            aes.KeySize = keySize;
            aes.GenerateKey();
            return aes.Key;
        }

        /// <summary>
        /// Generate a random IV
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateAesIV()
        {
            using var aes = Aes.Create();
            return aes.IV;
        }

        /// <summary>
        /// Encrypt a byte array input using AES symatric key with CBC mode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] EncryptAES(this byte[] input, byte[] key, byte[] iv)
        {
            // Ref: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.aesmanaged?view=net-8.0&redirectedfrom=MSDN
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Create an encryptor to perform the stream transform.
            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // Create the streams used for encryption.
            using MemoryStream msEncrypt = new MemoryStream();
            using CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            csEncrypt.Write(input, 0, input.Length);
            csEncrypt.FlushFinalBlock();
            return msEncrypt.ToArray();
        }

        /// <summary>
        /// Decrypt a byte array using AES symatric key with CBC mode
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] DecryptAES(this byte[] input, byte[] key, byte[] iv)
        {   
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Create a decryptor to perform the stream transform.
            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // Create the streams used for decryption.
            using MemoryStream msDecrypt = new MemoryStream(input);
            using CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using System.IO.MemoryStream msPlain = new System.IO.MemoryStream();
            csDecrypt.CopyTo(msPlain);
            return msPlain.ToArray();
        }

        /// <summary>
        /// Default Hash Length used in Argon2id hashing
        /// </summary>
        public const int DEFAULT_HASH_LENGTH = 32; // Corresponding to 44 Base64 characters length

        public const int DEFAULT_SALT_LENGTH = 16; // Corresponding to 24 Base64 characters length
        
        // These values are tested to have computation taken around 200-300 ms on test hardware:
        
        /// <summary>
        /// Default Hash Iteration used in Argon2id hashing
        /// </summary>
        public const int DEFAULT_ITERATIONS = 4;

        /// <summary>
        /// Default Hash Memory Size used in Argon2id hashing
        /// </summary>
        public const int DEFAULT_MEMORY_SIZE = 65536;

        /// <summary>
        /// Default Hash Degree of Parallelism used in Argon2id hashing
        /// </summary>
        public const int DEFAULT_DEGREE_OF_PARALLELISM = 8;

        /// <summary>
        /// Generate a random Salt byte array
        /// </summary>
        /// <param name="saltLength"></param>
        /// <returns></returns>
        public static byte[] GenerateSalt(int saltLength = DEFAULT_SALT_LENGTH)
        {
            byte[] salt = new byte[saltLength];
            new Random().NextBytes(salt);
            return salt;
        }

        /// <summary>
        /// Hash using Argon2id algorithm
        /// </summary>
        /// <param name="input"></param>
        /// <param name="salt"></param>
        /// <param name="hashLength"></param>
        /// <param name="iterations"></param>
        /// <param name="memorySize"></param>
        /// <param name="degreeOfParallelism"></param>
        /// <returns></returns>
        public static byte[] HashArgon2id(this byte[] input, byte[] salt,
            int hashLength = DEFAULT_HASH_LENGTH,
            int iterations = DEFAULT_ITERATIONS,
            int memorySize = DEFAULT_MEMORY_SIZE,
            int degreeOfParallelism = DEFAULT_DEGREE_OF_PARALLELISM
            )
        {
            using var hasher = new Argon2id(input);
            hasher.Salt = salt;
            hasher.Iterations = iterations;
            hasher.MemorySize = memorySize;
            hasher.DegreeOfParallelism = degreeOfParallelism;
            return hasher.GetBytes(hashLength);
        }

        /// <summary>
        /// Generate a random pair of RSA Public and Private Keys (default 2048 bits key size) in PEM format
        /// </summary>
        /// <param name="keySizeInBits"></param>
        /// <returns></returns>
        public static (string Public, string Private) GenerateRSAKeys(int keySizeInBits = 2048)
        {
            using var rsa = RSA.Create(keySizeInBits);
            return (rsa.ExportRSAPublicKeyPem(), rsa.ExportRSAPrivateKeyPem());
        }

        /// <summary>
        /// Generate a random pair of RSA Public and Private Keys (default 2048 bits key size) in PEM formats, including SPKI (for Public) and PKCS8 (for Private)
        /// </summary>
        /// <param name="keySizeInBits"></param>
        /// <returns></returns>
        public static (string PublicPem, string PrivatePem, string PublicSpki, string PrivatePkcs8) GenerateRSAKeysFull(int keySizeInBits = 2048)
        {
            using var rsa = RSA.Create(keySizeInBits);
            return (rsa.ExportRSAPublicKeyPem(), rsa.ExportRSAPrivateKeyPem(), rsa.ExportSubjectPublicKeyInfoPem(), rsa.ExportPkcs8PrivateKeyPem());
        }

        /// <summary>
        /// Encrypt using RSA Public Key.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="publicKey">Public Key in PEM format</param>
        /// <param name="padding">Padding used, default OAEP with SHA256 Hashing</param>
        /// <returns></returns>
        public static byte[] EncryptRSA(this byte[] input, string publicKey, RSAEncryptionPadding padding = null) 
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);
            return rsa.Encrypt(input, padding ?? RSAEncryptionPadding.OaepSHA256);
        }

        /// <summary>
        /// Decrypt using RSA Private Key.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="privateKey">Private Key in PEM format</param>
        /// <param name="padding">Padding used, default OAEP with SHA256 Hashing</param>
        /// <returns></returns>        
        public static byte[] DecryptRSA(this byte[] input, string privateKey, RSAEncryptionPadding padding = null)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            return rsa.Decrypt(input, padding ?? RSAEncryptionPadding.OaepSHA256);
        }

        /// <summary>
        /// Sign using RSA Private Key, with SHA256 Hashing and PKCS1 Padding as default
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey">Private Key in PEM format</param>
        /// <returns></returns>
        public static byte[] SignRSA(this byte[] data, string privateKey, 
            HashAlgorithmName hashAlgorithmName = new HashAlgorithmName())
        {
            if (hashAlgorithmName.Name == null)
                hashAlgorithmName = HashAlgorithmName.SHA256;

            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKey);
            return rsa.SignData(data, hashAlgorithmName, RSASignaturePadding.Pkcs1);
        }

        /// <summary>
        /// Verify using RSA Private Key, with SHA256 Hashing and PKCS1 Padding as default
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hash">Signature/Hash from RSA signing</param>
        /// <param name="publicKey">Public Key in PEM format</param>
        /// <returns></returns>
        public static bool VerifyRSA(this byte[] data, byte[] hash, string publicKey,
            HashAlgorithmName hashAlgorithmName = new HashAlgorithmName())
        {
            if (hashAlgorithmName.Name == null)
                hashAlgorithmName = HashAlgorithmName.SHA256;

            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);
            return rsa.VerifyData(data, hash, hashAlgorithmName, RSASignaturePadding.Pkcs1);
        }
    }
}
