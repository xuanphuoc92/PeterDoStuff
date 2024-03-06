using FluentAssertions;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _02_StringAndBytesExtensions_Test
    {
        [TestMethod]
        public void _01_UTF8()
        {
            "Hello"
                .ToByteArray()
                .ToUTF8String()
                .Should().Be("Hello");
        }

        [TestMethod]
        public void _02_Base64()
        {
            var base64 = "Hello"
                .ToByteArray()
                .ToBase64String();
            
            base64.Should().Be("SGVsbG8=");

            base64
                .ToByteArrayAsBase64String()
                .ToUTF8String()
                .Should().Be("Hello");
        }

        [TestMethod]
        public void _03_HexString()
        {
            var hexString = "Hello"
                .ToByteArray()
                .ToHexString();

            hexString.ToLower().Should().Be("48656c6c6f");

            hexString
                .ToByteArrayAsHexString()
                .ToUTF8String()
                .Should().Be("Hello");
        }

        [TestMethod]
        public void _04_JoinString()
        {
            var input = new string[] { "Alice", "Bob" };
            input.Join(", ").Should().Be("Alice, Bob");
        }
    }
}
