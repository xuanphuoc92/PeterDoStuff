using FluentAssertions;
using PeterDoStuff.Tools.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Tools
{
    [TestClass]
    public class _01_Schema
    {
        [TestMethod]
        public void _01_FuncDependency()
        {
            var fd1 = new FuncDependency("A, B", "A");
            var fd2 = new FuncDependency("A, B", "C");
            var fd3 = new FuncDependency("A, B", "A, C");

            fd1.LeftString.Should().Be("A, B");
            fd1.RightString.Should().Be("A");
            fd1.Left.Should().HaveCount(2);
            fd1.Right.Should().HaveCount(1);

            fd1.IsTrivial().Should().BeTrue();
            fd1.IsNonTrivial().Should().BeFalse();
            fd1.IsCompletelyNonTrivial().Should().BeFalse();

            fd2.IsTrivial().Should().BeFalse();            
            fd2.IsCompletelyNonTrivial().Should().BeTrue();

            fd3.IsTrivial().Should().BeFalse();            
            fd3.IsCompletelyNonTrivial().Should().BeFalse();
        }

        [TestMethod]
        public void _02_MultiValDependency()
        {
            var mv1 = new MultiValDependency("A, B", "C, D");
            var mv2 = new MultiValDependency("A, B", "A");

            mv1.IsTrivial().Should().BeTrue();
            mv1.IsNonTrivial().Should().BeFalse();

            mv2.IsTrivial().Should().BeTrue();

            mv1.IsTrivial("E").Should().BeFalse();
            mv1.IsTrivial("A", "E").Should().BeFalse();
        }
    }
}
