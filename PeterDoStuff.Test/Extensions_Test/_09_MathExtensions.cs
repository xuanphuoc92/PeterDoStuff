using FluentAssertions;
using PeterDoStuff.Extensions;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _09_MathExtensions
    {
        [TestMethod]
        public void _01_Cap()
        {
            ((double)0).Cap(0, 1).Should().Be(0);
            ((double)1).Cap(0, 1).Should().Be(0);
            ((double)0.5).Cap(0, 1).Should().Be(0.5);
            ((double)-0.1).Cap(0, 1).Should().Be(0.9);
        }
    }
}
