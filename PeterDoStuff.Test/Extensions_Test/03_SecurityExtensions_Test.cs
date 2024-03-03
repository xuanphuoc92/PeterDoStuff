using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
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
    }
}
