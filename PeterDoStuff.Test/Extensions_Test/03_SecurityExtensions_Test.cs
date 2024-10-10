using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
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
                .HashSHA256()
                .ToHexString().ToLower()
                .Should().Be("185f8db32271fe25f561a6fc938b2e264306ec304eda518007d1764826381969");
        }

        [TestMethod]
        public void _02_SHA512Hash()
        {
            "Hello"
                .ToByteArray()
                .HashSHA512()
                .ToHexString().ToLower()
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

            var encrypted1 = input1.ToByteArray().EncryptAES(key, iv);
            var encrypted2 = input2.ToByteArray().EncryptAES(key, iv);

            encrypted1.ToHexString().WriteToConsole("encrypted1");
            encrypted2.ToHexString().WriteToConsole("encrypted2");

            var decrypted1 = encrypted1.DecryptAES(key, iv);
            var decrypted2 = encrypted2.DecryptAES(key, iv);
            
            decrypted1.ToUTF8String().WriteToConsole("decrypted1");
            decrypted2.ToUTF8String().WriteToConsole("decrypted2");

            decrypted1.ToUTF8String().Should().Be(input1);
            decrypted2.ToUTF8String().Should().Be(input2);
        }

        [TestMethod]
        public void _05_AES_CrossCheck()
        {
            var key = "2767FE49F82C0524B6E25070EFFD2898DBA24E086BB109D87CA92BB8D48D97FD".ToByteArrayAsHexString();
            var iv = "AB0F82F30AC3720C4CA23422BC551F82".ToByteArrayAsHexString();
            "Hello"
                .ToByteArray()
                .EncryptAES(key, iv)
                .ToHexString()
                .Should().Be("8DEC8F99BCE15F426455112CB3584FB7");
        }

        [TestMethod]
        public void _06_Argon2id()
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

            "Hello".ToByteArray()
                .HashArgon2id(
                    salt: "fcf07dba2e0af8e5d5e27b14c2383ee9".ToByteArrayAsHexString(),
                    iterations: 1,
                    memorySize: 10,
                    degreeOfParallelism: 1)
                .ToHexString().ToLower()
                .Should().Be("720fcae9a394be95920af6711c6477c54df6619ebf6a7839cbfee6c1913d0d09");
        }

        [TestMethod]
        public void _07_RSA()
        {
            (string publicKey, string privateKey) = SecurityExtensions.GenerateRSAKeys();
            TestRsa(publicKey, privateKey);

            var fullKeySet = SecurityExtensions.GenerateRSAKeysFull();                        

            TestRsa(fullKeySet.PublicPem, fullKeySet.PrivatePem);
            TestRsa(fullKeySet.PublicSpki, fullKeySet.PrivatePkcs8);
        }

        private static void TestRsa(string publicKey, string privateKey)
        {
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

            // RSA Encryption of same message must be different due to random padding
            encrypted1.Should().NotBe(encrypted2);

            var decrypted1 = encrypted1.ToByteArrayAsBase64String().DecryptRSA(privateKey).ToUTF8String();
            var decrypted2 = encrypted2.ToByteArrayAsBase64String().DecryptRSA(privateKey).ToUTF8String();

            decrypted1.Should().Be(input);
            decrypted2.Should().Be(input);

            decrypted1.WriteToConsole("decrypted1");
            decrypted2.WriteToConsole("decrypted2");

            var hash1 = "Hello1".ToByteArray().SignRSA(privateKey);
            var hash2 = "Hello2".ToByteArray().SignRSA(privateKey);
            var hash1Same = "Hello1".ToByteArray().SignRSA(privateKey);

            hash1.ToBase64String().Should().Be(hash1Same.ToBase64String());
            hash2.ToBase64String().Should().NotBe(hash1Same.ToBase64String());

            "Hello1".ToByteArray().VerifyRSA(hash1, publicKey).Should().BeTrue();
            "Hello2".ToByteArray().VerifyRSA(hash2, publicKey).Should().BeTrue();

            "Hello1".ToByteArray().VerifyRSA(hash2, publicKey).Should().BeFalse();
            "Hello2".ToByteArray().VerifyRSA(hash1, publicKey).Should().BeFalse();

            Action errorAction = () => "Hello1".ToByteArray().SignRSA(publicKey);
            errorAction.Should().Throw<Exception>();

            "Hello1".ToByteArray().VerifyRSA(hash1, privateKey).Should().BeTrue();
            "Hello1".ToByteArray().VerifyRSA(hash2, privateKey).Should().BeFalse();
        }

        [TestMethod]
        public void _08_RSA_CrossCheck()
        {
            string publicKey = @"-----BEGIN RSA PUBLIC KEY-----
MIIBCgKCAQEA9CLOYPGf7khzA8rTEqdhMYRR8GDtiXvYLPx/HrHSkCybiTRK3Hr5
Zvmb6vv1N4g8Sm9/AdQMRplRK013IN2N2VBFDvjAuGUOK7Pqr0aq3UrfTi6Eq9SP
3gDDqNZx4bKfyWuJV+vEqep8jqHCrxr0pjAsm6fo4zo1pQS7U4aiiCFofJItHkN/
UrFT8KV0f5D1uArzORv4ogewcKkhryC7AU3TkaGoTGoHN5C/nLr9V2yxUTIwh1FY
9W+mTkBi6Hfq4l1FXo913h0ZM0x5mxQQMmj2I8QHP+JQVSnVW9cYiAR8S58tIaoV
LXa3mwtbr9rVAOk5XKDgZyPIZ0yDUMAdNQIDAQAB
-----END RSA PUBLIC KEY-----";
            string privateKey = @"-----BEGIN RSA PRIVATE KEY-----
MIIEowIBAAKCAQEA9CLOYPGf7khzA8rTEqdhMYRR8GDtiXvYLPx/HrHSkCybiTRK
3Hr5Zvmb6vv1N4g8Sm9/AdQMRplRK013IN2N2VBFDvjAuGUOK7Pqr0aq3UrfTi6E
q9SP3gDDqNZx4bKfyWuJV+vEqep8jqHCrxr0pjAsm6fo4zo1pQS7U4aiiCFofJIt
HkN/UrFT8KV0f5D1uArzORv4ogewcKkhryC7AU3TkaGoTGoHN5C/nLr9V2yxUTIw
h1FY9W+mTkBi6Hfq4l1FXo913h0ZM0x5mxQQMmj2I8QHP+JQVSnVW9cYiAR8S58t
IaoVLXa3mwtbr9rVAOk5XKDgZyPIZ0yDUMAdNQIDAQABAoIBACY3aJ0OVd3EI5T9
ZAswfmt07iq10ZlK9K0eHXwdk/uTsAlLUUiwS2hOPJUNOfamceGpEHWlzwIiL+2a
Y8KWTAUvpo/QasKytwQqVtt3MXoQpWIksAoB7T9wWTCN2SfegVrTZZ2Iv7Fljnf5
ZHNqwc9eDS2UnEUtsIYTdot9sOWkHuRC9/k9fZcnyr96dQ+RFUbHGOAp3sPKl05Y
mfbeJDNBBMjQtB7FsVC4Tay567gX+gw1uOixT4w/wheQ67AXV2vAWNAp+ipjxTET
eIa03LUTuzwq1TKgyLr7In01kTrKWq3QyJh9XOLqJ/Fcjl2cv3GYVw05mrBxLaBH
MvOnaGUCgYEA951rCWgC/RkuPX2Ek46E/YVqsI8BkK5rHwtRL7e84/c4L1FMe+ro
nM/aS7p0ouIJsjLrGUS5uCIaCH/qTeSz1IoYsKOGhbF4WMQCFNPmeyA8Iv3DbJY7
9AWuiJyogBgwK2dqwbzm8Vtg2d/iFIXKBOvoJAZTMJd7PSYfSERW05cCgYEA/Gc6
mPbSEV4vOIE43uUQvVXgU0COKwXrMdMkYhJWXQGFG9vINiaelZb1pooGD78usKjA
Y0igKAkggaZvWpXYHMFtty3NXY/vmSlUL3j2ph2q9TzbtqY06YEeL5E9qBLpg9FU
BoyDuSL7976xNSS/aZaxqh822zRZkFVxhfAt/xMCgYEAl7sHnvD0e+FVO2rRtZWs
mqmJkf1fiSfIDnLh4eqmPc78x8n6oyh0N7sKWkM8O59lL6QR+h7p6xJCf5jam7ac
F44zyPG7eeshAsvBNsAOSL6c+xgjC0QYItTkeWP9wNiRk9dATM2Teqxy8a9GGytM
eq2QR5r0mR8J7pOQsbjmt6sCgYBaI8+kgzlAZJ9+kX+q8qmIxJuJf9uF9+Yn9Bzh
PWVEb9+GtHYLCL3H9JKkOFSz2PYmtw2GJ5Cy25eVVcgc1LjKhDXXnm7iRB4wV7cu
AQgtCRld3a8lyrPI1IjgOgGH/cERh3d3o0UDoD/WGW8V6JrHbb7jX6RqooZ96lXR
nJYAqwKBgGpSk/oNVNlQPbE5y7P7w0qXnMDEwjxV/TNmrt4XepPY2wlhbUi66KG1
lYCoShUzCQ7CM2UdvkfARI7E5uLBJ+d4ODJQl19WghNQnnmYYg6fJzFI5F50226w
ZyLOGR55QuJgCvj6GedbhAltgnhOzRCjSSea3e1KpAhao3Jr1lxC
-----END RSA PRIVATE KEY-----";
            string encryptedHex = "d76d83ff140339b267617d377140f2c37745cefb0d62a12d80fcf2dd16d5eab36fd20aae87b52cd7e29e4438b5773908573f23b99f294ba33428f5ea9cabd0ce1ef6a84fd3e5318307df25d380728da06f6b6b0aa5fc41ba9392f33fc90371264fcf11e194d5db47017aafbb758a5c2c2b2bb7214081aa9efb8578f794a4689ba72e9f1723d3bf683ad4c33dad8f019c89a05a54c1e1fbe39f1662b1a755a0fbfdf3574bb14e7ac550272c21d1ea5c04d8877782b6eb7ab943de890046732169ec90efcd9b7fc18ddd955d38168b437f26bffef836ab2b66b3ea7110a369026c68099b0672eb42940579d83f0b49eab14af84aba3b38d2eabe69882d25b0e318";
            string signatureHex = "8846de279def06624f9bf3cf57b2916c708deea2813856ceaccf946a5cc4c48afe232407095929eca235f434fc9df6e150ea4e0ce70ec75bf623aafeb7e89a1c18d461f34ec00d300eee5720b3f9e3159e0305ee97c26552c28b0a8540feca9f2b0f5fe26614bdbecbd9cab13c13c01bd87395b56f4d68bebfa24230d36057df6a04b18e4d6b7e60684f374621c3f9366eedcb54eda6b291f3f89c971e74ebe2e6f3c93b3c5c8a94a21bf00b26fc0f2903ccd4953d26a164b91e329b04b8ee48c6b4064c0eaeee542c48d6b49b6e07ef8d6d800aff8e147c4c65883653e2d6265ef4753c8906499e4404578acf53c9f19bedb9af1abdfd331f9a3304dadccc34";
            string input = "Hello";

            string encryptedRandomHex = input.ToByteArray().EncryptRSA(publicKey, RSAEncryptionPadding.Pkcs1).ToHexString();

            encryptedHex.Should().NotBe(encryptedRandomHex);

            string decrypted1 = encryptedHex.ToByteArrayAsHexString().DecryptRSA(privateKey, RSAEncryptionPadding.Pkcs1).ToUTF8String();
            string decrypted2 = encryptedRandomHex.ToByteArrayAsHexString().DecryptRSA(privateKey, RSAEncryptionPadding.Pkcs1).ToUTF8String();

            decrypted1.Should().Be(input);
            decrypted2.Should().Be(input);

            var sha512Hash = HashAlgorithmName.SHA512;

            input.ToByteArray().SignRSA(privateKey, sha512Hash).ToHexString().ToLower().Should().Be(signatureHex);
            input.ToByteArray().VerifyRSA(signatureHex.ToByteArrayAsHexString(), publicKey, sha512Hash).Should().BeTrue();
        }

        [TestMethod]
        public void _09_HMACSHA()
        {
            var input = "Hello".ToByteArray();
            var key = "MyKey".ToByteArray();

            var hash256 = input.HashHMACSHA256(key).ToHexString().ToLower();
            var hash512 = input.HashHMACSHA512(key).ToHexString().ToLower();

            hash256.Should().Be("fd2648d5187786cfcc454d9598e62b90c3f5f3402debc0201a777a02e8fb3aa1");
            hash512.Should().Be("887216b50d8bc0dd5ecab02bb7fe7e70a3b17e4810101a277834ba2790aea9a8a28b1894021dd94184f68e4008739d2306f645b96b78e485703a772417e0695c");
        }
    }
}
