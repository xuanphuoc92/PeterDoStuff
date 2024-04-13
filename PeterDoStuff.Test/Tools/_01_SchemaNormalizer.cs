using FluentAssertions;
using PeterDoStuff.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Tools
{
    [TestClass]
    public class _01_SchemaNormalizer
    {
        [TestMethod]
        public void _01_AddDependency()
        {
            var normalizer = SchemaNormalizer.New()
                .AddDependency(["A", "B"], ["C"])
                .AddDependency(["A"], ["D"])
                .AddDependency(["A"], ["C", "D"]);

            normalizer.GetAllAttributes().Should().HaveCount(4);

            normalizer.Dependencies[0].LeftString.Should().Be("A, B");
            normalizer.Dependencies[0].RightString.Should().Be("C");
        }
    }
}
