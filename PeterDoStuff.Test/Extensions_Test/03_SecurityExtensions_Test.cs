using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _03_SecurityExtensions_Test
    {
        [TestMethod]
        public void _01_SHA256Hash()
        {
            "Hello"
                .ToByteArray()
                .ToSHA256()
                .ToHexString()
                .Should().Be("185f8db32271fe25f561a6fc938b2e264306ec304eda518007d1764826381969");
        }

        [TestMethod]
        public void _02_SHA512Hash()
        {
            "Hello"
                .ToByteArray()
                .ToSHA512()
                .ToHexString()
                .Should().Be("3615f80c9d293ed7402687f94b22d58e529b8cc7916f8fac7fddf7fbd5af4cf777d3d795a7a00a16bf7e7f3fb9561ee9baae480da9fe7a18769e71886b03f315");
        }

        [TestMethod]
        public void _03_AES_256()
        {
            var key = SecurityExtensions.GenerateAesKey();
            TestAES(key);
        }

        [TestMethod]
        public void _04_AES_128()
        {
            var key = SecurityExtensions.GenerateAesKey(128);
            TestAES(key);
        }

        private static void TestAES(byte[] key)
        {
            var iv = SecurityExtensions.GenerateAesIV();

            key.ToHexString().WriteToConsole("key");
            iv.ToHexString().WriteToConsole("iv");

            var input1 = "Hello1";
            var input2 = "Hello2";

            var encrypted1 = input1.EncryptAES(key, iv);
            var encrypted2 = input2.EncryptAES(key, iv);

            encrypted1.ToHexString().WriteToConsole("encrypted1");
            encrypted2.ToHexString().WriteToConsole("encrypted2");

            var decrypted1 = encrypted1.DecryptAES(key, iv);
            var decrypted2 = encrypted2.DecryptAES(key, iv);
            
            decrypted1.WriteToConsole("decrypted1");
            decrypted2.WriteToConsole("decrypted2");

            decrypted1.Should().Be(input1);
            decrypted2.Should().Be(input2);
        }

        [TestMethod]
        public void _05_Argon2id()
        {
            var salt1 = SecurityExtensions.GenerateSalt();
            var salt2 = SecurityExtensions.GenerateSalt();

            salt1.ToHexString().WriteToConsole("salt1");
            salt2.ToHexString().WriteToConsole("salt2");
            salt1.ToHexString().Should().NotBe(salt2.ToHexString());

            var hash1 = "Hello1".ToByteArray().HashArgon2id(salt1, iterations: 1, memorySize: 10, degreeOfParallelism: 1).ToHexString();
            var hash2 = "Hello2".ToByteArray().HashArgon2id(salt2, iterations: 1, memorySize: 10, degreeOfParallelism: 1).ToHexString();

            var hash1Same = "Hello1".ToByteArray().HashArgon2id(salt1, iterations: 1, memorySize: 10, degreeOfParallelism: 1).ToHexString();
            var hashMix = "Hello1".ToByteArray().HashArgon2id(salt2, iterations: 1, memorySize: 10, degreeOfParallelism: 1).ToHexString();

            hash1.WriteToConsole("hash1");
            hash2.WriteToConsole("hash2");
            hashMix.WriteToConsole("hashMix");

            hash1.Should().NotBe(hash2);
            hash1.Should().NotBe(hashMix);
            hash1.Should().Be(hash1Same);
            hash2.Should().NotBe(hashMix);

            Stopwatch stopwatch = Stopwatch.StartNew();
            "Hello1".ToByteArray().HashArgon2id(salt1);
            stopwatch.Stop();
            stopwatch.ElapsedMilliseconds.ToString().WriteToConsole("Standard Hash Time");
            stopwatch.ElapsedMilliseconds.Should().BeGreaterThan(200);
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(500);
        }

        [TestMethod]
        public void _06_RSA()
        {
            (string publicKey, string privateKey) = SecurityExtensions.GenerateRSAKeys();
            publicKey.WriteToConsole("public");
            privateKey.WriteToConsole("private");

            // Public Key is a subset of Private Key, so should have lower length
            publicKey.Length.Should().BeLessThan(privateKey.Length);

            var input = "Hello";
            input.WriteToConsole("input");

            var encrypted1 = input.ToByteArray().EncryptRSA(publicKey).ToBase64String();
            var encrypted2 = input.ToByteArray().EncryptRSA(publicKey).ToBase64String();

            encrypted1.WriteToConsole("encrypted1");
            encrypted2.WriteToConsole("encrypted2");

            // Encryption of RSA must be different 
            encrypted1.Should().NotBe(encrypted2);

            var decrypted1 = encrypted1.ToByteArrayAsBase64().DecryptRSA(privateKey).ToUTF8String();
            var decrypted2 = encrypted2.ToByteArrayAsBase64().DecryptRSA(privateKey).ToUTF8String();

            decrypted1.Should().Be(input);
            decrypted2.Should().Be(input);

            decrypted1.WriteToConsole("decrypted1");
            decrypted2.WriteToConsole("decrypted2");
        }
    }
}
