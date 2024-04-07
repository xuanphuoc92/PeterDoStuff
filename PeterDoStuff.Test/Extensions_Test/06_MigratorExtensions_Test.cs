using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

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

            public decimal DefaultDecimal { get; set; }

            [DecimalPrecisionScale(20, 8)]
            public decimal? CustomDecimal { get; set; }

            public int Number { get; set; }

            public float Longitude { get; set; }
            public double Latitude { get; set; }

            public DateTime CreatedTime { get; set; }

            [DateOnly]
            public DateTime? CreatedDate { get; set; }

            public TestEnum DefaultEnum { get; set; }
            
            [Column(TypeName = "nvarchar(7)")]
            public TestEnum StringEnum { get; set; }

            [NotMapped]
            public string NoMapColumn { get; set; }
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
            context.GetMigrator().GetCreateSql().Verify();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _02_DropSql()
        {
            using var context = GetTestContext();
            context.GetMigrator().GetDropSql().Verify();
        }

        [TestMethod]
        public async Task _03_ReadWrite()
        {
            Guid defaultId, customId;

            using (var context = GetTestContext())
            {
                var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
                var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

                await context.Database.ExecuteSqlAsync(dropSql);
                await context.Database.ExecuteSqlAsync(createSql);

                var defaultEntity = new TestEntity1();
                var customEntity = new TestEntity2();

                customEntity.Name = "Test Name";
                customEntity.Description = "Test Description";
                customEntity.StandardHash = "Test Hash".ToByteArray();
                customEntity.BigHash = "Test Big Hash".ToByteArray();
                customEntity.DefaultDecimal = 1;
                customEntity.CustomDecimal = 2.2m;
                customEntity.Number = 3;
                customEntity.Longitude = 1.234f;
                customEntity.Latitude = 5.678f;
                customEntity.CreatedTime = new DateTime(2024, 01, 01, 01, 01, 01);
                customEntity.CreatedDate = new DateTime(2024, 01, 01);
                customEntity.DefaultEnum = TestEnum.Special;
                customEntity.StringEnum = TestEnum.Custom;
                customEntity.NoMapColumn = "No Map Test";

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
                defaultEntity.Description.Should().BeNull();
                defaultEntity.StandardHash.Should().BeNull();
                defaultEntity.BigHash.Should().BeNull();
                defaultEntity.DefaultDecimal.Should().Be(0);
                defaultEntity.CustomDecimal.Should().BeNull();
                defaultEntity.Number.Should().Be(0);
                defaultEntity.Longitude.Should().Be(0);
                defaultEntity.Latitude.Should().Be(0);
                defaultEntity.CreatedTime.Should().Be(default);
                defaultEntity.CreatedDate.Should().BeNull();
                defaultEntity.DefaultEnum.Should().Be(TestEnum.Default);
                defaultEntity.StringEnum.Should().Be(TestEnum.Default);
                defaultEntity.NoMapColumn.Should().BeNull();

                customEntity.Name.Should().Be("Test Name");
                customEntity.Description.Should().Be("Test Description");
                customEntity.StandardHash.ToUTF8String().Should().Be("Test Hash");
                customEntity.BigHash.ToUTF8String().Should().Be("Test Big Hash");
                customEntity.DefaultDecimal.Should().Be(1);
                customEntity.CustomDecimal.Should().Be(2.2m);
                customEntity.Number.Should().Be(3);
                customEntity.Longitude.Should().Be(1.234f);
                customEntity.Latitude.Should().Be(5.678f);
                customEntity.CreatedTime.Should().Be(new DateTime(2024, 01, 01, 01, 01, 01));
                customEntity.CreatedDate.Should().Be(new DateTime(2024, 01, 01));
                customEntity.DefaultEnum.Should().Be(TestEnum.Special);
                customEntity.StringEnum.Should().Be(TestEnum.Custom);
                customEntity.NoMapColumn.Should().BeNull();
            }
        }
    }
}
