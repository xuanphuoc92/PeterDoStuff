using FluentAssertions;
using PeterDoStuff.Database;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _01_TypeExtensions_Test
    {
        private class TestClass
        {
            public int Number { get; set; } = 1;
            public string Name { get; set; } = "Alice";
        }

        private ExpandoObject GetTestDynamicObject()
        {
            dynamic obj = new ExpandoObject();
            obj.Number = 1;
            obj.Name = "Alice";
            return obj;
        }

        [TestMethod]
        public void _01_GetPropertyNames()
        {
            TestClass obj = new TestClass();
            var properties = obj.GetPropertyNames();
            properties.Should().HaveCount(2);
            properties.Should().Contain("Number");
            properties.Should().Contain("Name");
        }

        [TestMethod]
        public void _02_GetPropertyNames_Dynamic()
        {
            dynamic obj = GetTestDynamicObject();
            var properties = ((object)obj).GetPropertyNames();
            properties.Should().HaveCount(2);
            properties.Should().Contain("Number");
            properties.Should().Contain("Name");
        }

        [TestMethod]
        public void _03_GetPropertyValue()
        {
            TestClass obj = new TestClass();
            int number = obj.GetPropertyValue("Number").As<int>();
            string name = obj.GetPropertyValue("Name").As<string>();
            number.Should().Be(1);
            name.Should().Be("Alice");
        }

        [TestMethod]
        public void _04_GetPropertyValue_Dynamic()
        {
            dynamic obj = GetTestDynamicObject();
            int number = ((object)obj).GetPropertyValue("Number").As<int>();
            string name = ((object)obj).GetPropertyValue("Name").As<string>();
            number.Should().Be(1);
            name.Should().Be("Alice");
        }

        [TestMethod]
        public async Task _05_DapperDynamic()
        {
            using var db = new MemoryDb();
            using var conn = db.Open();
            var result = await conn.QueryAsync(SqlCommand.SAMPLE_TEST_SQL);
            dynamic obj = result.First();            

            var properties = ((object)obj).GetPropertyNames();
            properties.Should().HaveCount(2);
            properties.Should().Contain("Number");
            properties.Should().Contain("Name");

            int number = ((object)obj).GetPropertyValue("Number").As<int>();
            string name = ((object)obj).GetPropertyValue("Name").As<string>();
            number.Should().Be(1);
            name.Should().Be("Alice");
        }
    }
}
