using FluentAssertions;
using PeterDoStuff.Attributes;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _07_ValidatorExtensions_Test
    {
        private void Print(List<ValidationResult> validationResult)
        {
            foreach (var validationResultItem in validationResult)
                validationResultItem.ErrorMessage.WriteToConsole();
        }

        private class Entity1
        {
            [MaxLength(100)]
            public string Name { get; set; }

            public string Description { get; set; }
        }

        [TestMethod]
        public void _01_MaxLengthString_Test()
        {
            var entity = new Entity1();
            entity.IsValid().Should().BeTrue();

            entity.Name = new string('t', 100);
            entity.Description = new string('t', 1000);
            entity.IsValid().Should().BeTrue();

            entity.Name = new string('t', 101);
            entity.IsValid(out var validationResult).Should().BeFalse();
            Print(validationResult);
        }

        private class Entity2
        {
            [MaxLength(32)]
            public byte[] StandardHash { get; set; }

            public byte[]? BigHash { get; set; }
        }

        [TestMethod]
        public void _02_MaxLengthHash_Test()
        {
            var entity = new Entity2();
            entity.IsValid().Should().BeTrue();

            entity.StandardHash = SecurityExtensions.GenerateSalt(32);
            entity.BigHash = SecurityExtensions.GenerateSalt(32);
            entity.IsValid().Should().BeTrue();

            entity.StandardHash = SecurityExtensions.GenerateSalt(33);
            entity.BigHash = SecurityExtensions.GenerateSalt(33);
            entity.IsValid(out var validationResult).Should().BeFalse();
            Print(validationResult);
        }

        private class Entity3
        {
            [DecimalPrecisionScale(6,2)]
            public decimal? CustomDecimal { get; set; }

            public decimal? NormalDecimal { get; set; }
        }

        [TestMethod]
        public void _03_DecimalPrecisionScale_Test()
        {
            var entity = new Entity3();
            entity.IsValid().Should().BeTrue();

            entity.CustomDecimal = 1.23m;
            entity.NormalDecimal = 1.23m;
            entity.IsValid().Should().BeTrue();

            entity.CustomDecimal = 1.234m;
            entity.NormalDecimal = 1.234m;
            entity.IsValid(out var validationResult).Should().BeFalse();
            Print(validationResult);

            entity.CustomDecimal = 1234m;
            entity.NormalDecimal = 1234m;
            entity.IsValid().Should().BeTrue();

            entity.CustomDecimal = 12345m;
            entity.NormalDecimal = 12345m;
            entity.IsValid(out validationResult).Should().BeFalse();
            Print(validationResult);
        }

        private class Entity4
        {
            [DateOnly]
            public DateTime? DateOnly{ get; set;}

            public DateTime? DateTime { get; set; }
        }

        [TestMethod]
        public void _04_DateOnly_Test()
        {
            var entity = new Entity4();
            entity.IsValid().Should().BeTrue();

            entity.DateOnly = DateTime.Today;
            entity.DateTime = DateTime.Today;
            entity.IsValid().Should().BeTrue();

            entity.DateOnly = DateTime.Today.AddMinutes(1);
            entity.DateTime = DateTime.Today.AddMinutes(1);
            entity.IsValid(out var validationResult).Should().BeFalse();
            Print(validationResult);
        }
    }
}
