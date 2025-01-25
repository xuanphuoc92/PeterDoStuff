using ApprovalTests.Reporters;
using FluentAssertions;
using FluentAssertions.Extensions;
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

        [TestMethod]
        public void _05_IsNullOrEmpty()
        {
            string input = "Hello";
            input.IsNullOrEmpty().Should().BeFalse();

            input = "";
            input.IsNullOrEmpty().Should().BeTrue();

            input = null;
            input.IsNullOrEmpty().Should().BeTrue();
        }

        private class TestModel
        {
            public string Name { get; set; } = "Apple";
            public DateTime From { get; set; } = 31.January(2020);
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _06_ToJson()
        {
            var model = new TestModel();
            var result = model.ToJson();
            result += "\n";
            result += model.ToJson(beautify: true);
            result.Verify();
        }

        [TestMethod]
        public void _07_FromJson()
        {
            var model = new TestModel();
            model.Name = "Orange";
            model.From = 28.February(2020);

            static void Assert(string json)
            {
                var deserializedModel = json.FromJson<TestModel>();
                deserializedModel.Name.Should().Be("Orange");
                deserializedModel.From.Should().Be(28.February(2020));
            }

            Assert(model.ToJson());
            Assert(model.ToJson(beautify: true));
        }
    }
}
