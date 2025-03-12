using FluentAssertions;
using PeterDoStuff.Extensions;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _09_Math
    {
        [TestMethod]
        public void _01_Cap()
        {
            double angle = -181;
            angle.Cap(-180, 180).Should().Be(179);
            angle = 180;
            angle.Cap(-180, 180).Should().Be(-180);
        }
    }
}
