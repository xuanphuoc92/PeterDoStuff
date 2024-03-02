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
        public void _01_SHA256Hash()
        {
            "Hello"
                .ToByteArray()
                .ToSHA256()
                .ToHexString()
                .Should().Be("185f8db32271fe25f561a6fc938b2e264306ec304eda518007d1764826381969");
        }
    }
}
