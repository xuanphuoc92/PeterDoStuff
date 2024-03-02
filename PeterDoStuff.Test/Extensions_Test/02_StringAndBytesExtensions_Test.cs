using FluentAssertions;
using PeterDoStuff.Extensions;
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
        public void _01_MD5Hash()
        {
            "Hello"
                .ToByteArray()
                .ToMD5Hash()
                .ToHexString()
                .Should().Be("8b1a9953c4611296a827abf8c47804d7");
        }
    }
}
