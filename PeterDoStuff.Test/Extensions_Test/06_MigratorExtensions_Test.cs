using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Extensions;
using PeterDoStuff.Identity;
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
    public class _06_MigratorExtensions_Test
    {
        private class TestContext : DbContext
        {
            public TestContext(DbContextOptions<TestContext> options) : base(options) { }
            public DbSet<TestEntity> __TestTable__ => Set<TestEntity>();
        }

        private class TestEntity
        {
            public Guid Id { get; set; }
            
            [MaxLength(100)]
            public string Name { get; set; }
            public string? Description { get; set; }

            [MaxLength(32)]
            public byte[]? StandardHash { get; set; }

            public byte[]? BigHash { get; set; }

            public decimal? DefaultDecimal { get; set; }

            [DecimalPrecisionScale(20, 8)]            
            public decimal? CustomDecimal { get; set; }

            public int Number { get; set; }

            public float Longitude { get; set; }
            public double Latitude { get; set; }

            public DateTime CreatedTime { get; set; }
            
            [DateOnly]
            public DateTime? CreatedDate { get; set; }

            public TestEnum DefaultEnum { get; set; }
            [NumberEnum]
            public TestEnum NumberEnum { get; set; }
        }

        private enum TestEnum
        {
            Default, Custom, Special
        }

        private TestContext GetTestContext()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new TestContext(options);
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _01_Sql()
        {
            using var context = GetTestContext();
            context.GetMigrator().Sql().Verify();
        }
    }
}
