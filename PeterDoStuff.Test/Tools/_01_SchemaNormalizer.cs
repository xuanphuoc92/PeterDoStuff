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
                .AddFuncDependency(["A", "B"], ["C"])
                .AddFuncDependency(["A"], ["D"])
                .AddFuncDependency(["A"], ["C", "D"]);

            normalizer.GetAllAttributes().Should().HaveCount(4);

            normalizer.FuncDependencies[0].LeftString.Should().Be("A, B");
            normalizer.FuncDependencies[0].RightString.Should().Be("C");
        }

        [TestMethod]
        public void _02_IsTrivial()
        {
            var normalizer = SchemaNormalizer.New()
                .AddFuncDependency(["A", "B"], ["A"])
                .AddFuncDependency(["A", "B"], ["C"])
                .AddFuncDependency(["A", "B"], ["A", "C"]);

            normalizer.FuncDependencies[0].IsTrivial().Should().BeTrue();
            normalizer.FuncDependencies[0].IsNonTrivial().Should().BeFalse();
            normalizer.FuncDependencies[0].IsCompletelyNonTrivial().Should().BeFalse();


            normalizer.FuncDependencies[1].IsTrivial().Should().BeFalse();
            normalizer.FuncDependencies[1].IsNonTrivial().Should().BeTrue();
            normalizer.FuncDependencies[1].IsCompletelyNonTrivial().Should().BeTrue();

            normalizer.FuncDependencies[2].IsTrivial().Should().BeFalse();
            normalizer.FuncDependencies[2].IsNonTrivial().Should().BeTrue();
            normalizer.FuncDependencies[2].IsCompletelyNonTrivial().Should().BeFalse();
        }
    }
}
