using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
            public DbSet<TestEntity1> __TestTable__ => Set<TestEntity1>();
            public DbSet<TestEntity2> __CustomTestTable__ => Set<TestEntity2>();

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // TODO: Make it into a custom DbContext
                modelBuilder.Entity<TestEntity1>()
                    .Property(e => e.DefaultEnum)
                    .HasConversion(new EnumToStringConverter<TestEnum>())
                    .HasColumnType("nvarchar(7)"); ;

                modelBuilder.Entity<TestEntity2>()
                    .Property(e => e.DefaultEnum)
                    .HasConversion(new EnumToStringConverter<TestEnum>())
                    .HasColumnType("nvarchar(7)"); ;
            }
        }

        private class TestEntity1 : TestEntity { }
        private class TestEntity2 : TestEntity { }

        private class TestEntity
        {
            public Guid Id { get; set; }

            [MaxLength(100)]
            public string Name { get; set; } = "";
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
        public void _01_CreateSql()
        {
            using var context = GetTestContext();
            context.GetMigrator().CreateSql().Verify();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _02_DropSql()
        {
            using var context = GetTestContext();
            context.GetMigrator().DropSql().Verify();
        }

        [TestMethod]
        public async Task _03_ReadWrite()
        {
            Guid defaultId, customId;

            using (var context = GetTestContext())
            {
                await context.GetMigrator().DropAsync();
                await context.GetMigrator().CreateAsync();

                var defaultEntity = new TestEntity1();
                var customEntity = new TestEntity2();

                customEntity.Name = "Test Name";

                context.__TestTable__.Add(defaultEntity);
                context.__CustomTestTable__.Add(customEntity);
                await context.SaveChangesAsync();

                defaultId = defaultEntity.Id;
                customId = customEntity.Id;
            }

            using (var context = GetTestContext()) 
            {
                var defaultEntity = context.__TestTable__.Find(defaultId);
                var customEntity = context.__CustomTestTable__.Find(customId);

                defaultEntity.Name.Should().BeEmpty();
                
                customEntity.Name.Should().Be("Test Name");
            }
        }
    }
}
