using ApprovalTests.Reporters;
using FluentAssertions;
using PeterDoStuff.Test.Extensions;
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

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _03_TheChase_Sample()
        {
            var theChase = new TheChase();
            
            // Arrange Input
            theChase.Schema.AddRange(["A", "B", "C", "D", "E", "G"]);
            theChase.Dependencies.Add(new FuncDependency("A,B","C"));
            theChase.Dependencies.Add(new MultiValDependency("A,B","A,E"));
            theChase.Dependencies.Add(new MultiValDependency("C,D","A,B"));
            string[] decom1 = ["A", "B", "C", "D", "G"];
            string[] decom2 = ["C", "D", "E"];

            // Act Process
            var result = theChase.ChaseDecompositions(decom1, decom2);

            // Assert Outputs
            theChase.Logs.Verify();
            result.Lossless1.Should().BeTrue();
            result.Lossless2.Should().BeTrue();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _04_TheChase_Positive()
        {
            var theChase = new TheChase();

            // Arrange Input
            theChase.Schema.AddRange(["A", "B", "C"]);
            theChase.Dependencies.Add(new FuncDependency("A", "B"));
            theChase.Dependencies.Add(new FuncDependency("B", "C"));

            // Act Process
            var result = theChase.ChaseDependency(new FuncDependency("A", "C"));

            // Assert Outputs
            theChase.Logs.Verify();
            result.Should().BeTrue();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _05_TheChase_Negative()
        {
            var theChase = new TheChase();

            // Arrange Input
            theChase.Schema.AddRange(["A", "B", "C"]);
            theChase.Dependencies.Add(new FuncDependency("A", "B"));
            theChase.Dependencies.Add(new FuncDependency("B", "C"));

            // Act Process
            var result = theChase.ChaseDependency(new FuncDependency("C", "A"));

            // Assert Outputs
            theChase.Logs.Verify();
            result.Should().BeFalse();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _06_TheChase_Cover1()
        {
            var theChase = new TheChase();

            // Arrange Input
            theChase.Schema.AddRange(["A", "B", "C"]);
            theChase.Dependencies.Add(new FuncDependency("A", "B"));
            theChase.Dependencies.Add(new FuncDependency("B", "C"));

            // Act Process
            var result = theChase.ChaseDependency(new MultiValDependency("B, C", "A"));

            // Assert Outputs
            theChase.Logs.Verify();
            result.Should().BeTrue();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _07_TheChase_Cover2()
        {
            var theChase = new TheChase();

            // Arrange Input
            theChase.Schema.AddRange(["A", "B", "C"]);
            theChase.Dependencies.Add(new FuncDependency("A", "B"));
            theChase.Dependencies.Add(new FuncDependency("B", "C"));

            // Act Process
            var result = theChase.ChaseDecompositions(["A"], ["B", "C"]);

            // Assert Outputs
            theChase.Logs.Verify();
            result.Lossless1.Should().BeFalse();
            result.Lossless2.Should().BeFalse();
        }
    }
}
