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
            public DbSet<TestEntity1> __TestTable__ { get; set; }
            public DbSet<TestEntity2> __CustomTestTable__ { get; set; }
        }

        private class TestEntity1 : TestEntity { }
        private class TestEntity2 : TestEntity { }

        private class TestEntity
        {
            public Guid Id { get; set; }

            [MaxLength(100)]
            public string Name { get; set; } = "";
            public String? Description { get; set; }

            [MaxLength(32)]
            public byte[]? StandardHash { get; set; }

            public byte[]? BigHash { get; set; }

            public decimal DefaultDecimal { get; set; }

            [DecimalPrecisionScale(20, 8)]
            public decimal? CustomDecimal { get; set; }

            public int Number { get; set; }
            public Int32 NumberInt32 { get; set; }
            public Int64 BigNumberInt64 { get; set; }

            public bool Flag1 { get; set; }
            public Boolean Flag2 { get; set; }

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

        private TContext GetTestContext<TContext>() where TContext : DbContext
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var options = new DbContextOptionsBuilder<TContext>()
                .UseSqlServer(connectionString)
                .Options;

            return (TContext)Activator.CreateInstance(typeof(TContext), options);
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _01_CreateSql()
        {
            using var context = GetTestContext<TestContext>();
            context.GetMigrator().GetCreateSql().Verify();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _02_DropSql()
        {
            using var context = GetTestContext<TestContext>();
            context.GetMigrator().GetDropSql().Verify();
        }        

        [TestMethod]
        public async Task _03_ReadWrite()
        {
            Guid defaultId, customId;

            using (var context = GetTestContext<TestContext>())
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
                customEntity.NumberInt32 = 4;
                customEntity.BigNumberInt64 = 5;
                customEntity.Flag1 = true;
                customEntity.Flag2 = true;
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

            using (var context = GetTestContext<TestContext>())
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
                defaultEntity.NumberInt32.Should().Be(0);
                defaultEntity.BigNumberInt64.Should().Be(0);
                defaultEntity.Flag1.Should().BeFalse();
                defaultEntity.Flag2.Should().BeFalse();
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
                customEntity.NumberInt32.Should().Be(4);
                customEntity.BigNumberInt64.Should().Be(5);
                customEntity.Flag1.Should().BeTrue();
                customEntity.Flag2.Should().BeTrue();
                customEntity.Longitude.Should().Be(1.234f);
                customEntity.Latitude.Should().Be(5.678f);
                customEntity.CreatedTime.Should().Be(new DateTime(2024, 01, 01, 01, 01, 01));
                customEntity.CreatedDate.Should().Be(new DateTime(2024, 01, 01));
                customEntity.DefaultEnum.Should().Be(TestEnum.Special);
                customEntity.StringEnum.Should().Be(TestEnum.Custom);
                customEntity.NoMapColumn.Should().BeNull();
            }
        }

        [TestMethod]
        public void _04_DescribeMapping()
        {
            using var context = GetTestContext<TestContext>();
            context.GetMigrator().DescribeMapping().WriteToConsole();
        }

        private class TestEntity3 : TestEntity;

        private class TestContextB : DbContext
        {
            public TestContextB(DbContextOptions<TestContextB> options) : base(options) { }

            public DbSet<TestEntity1> __TestTable__ { get; set; }
            public DbSet<TestEntity3> __AddedTestTable__ => Set<TestEntity3>();
        }

        [TestMethod]
        public void _05_AddTable()
        {
            using (var context = GetTestContext<TestContext>())
            {
                var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
                var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

                context.Database.ExecuteSql(dropSql);
                context.Database.ExecuteSql(createSql);
            }

            using (var context = GetTestContext<TestContextB>())
            {
                context.GetMigrator().UpdateSql().WriteToConsole();
            }
        }
    }
}
