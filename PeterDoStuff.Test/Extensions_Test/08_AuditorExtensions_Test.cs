using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Database;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _08_AuditorExtensions_Test
    {
        private class TestEntity
        {
            public int Id { get; set; }

            [MaxLength(100)]
            public string Name { get; set; }
        }

        private class TestLineItemEntity
        {
            public int Id { get; set; }
            public int AuditableEntityId { get; set; }
            [MaxLength(100)]
            public string Name { get; set; }
        }

        private class AuditableEntity : TestEntity
        {
            public List<TestLineItemEntity> LineItems { get; set; } = new();
        }

        private class NonAuditableEntity : TestEntity;

        private abstract class BaseContext : DbContext, IAuditableContext
        {
            public BaseContext(DbContextOptions options) : base(options) { }
            public DbSet<AuditEntity> Audit { get; set; }
        }

        private class AuditableContext : BaseContext
        {
            public AuditableContext(DbContextOptions<AuditableContext> options) : base(options) { }

            [Auditable]
            public DbSet<AuditableEntity> __AuditableTestTable__ { get; set; }

            public DbSet<NonAuditableEntity> __NonAuditableTestTable__ { get; set; }

            [Auditable]
            public DbSet<TestLineItemEntity> __AuditableTestLineItemTable__ { get; set; }

            public override int SaveChanges()
            {
                var auditSql = this.GetAuditor().GetAuditSql();
                this.Database.GetDbConnection().Execute(auditSql);
                return base.SaveChanges();
            }
        }

        private AuditableContext GetTestContext()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var options = new DbContextOptionsBuilder<AuditableContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new AuditableContext(options);
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _01_DropSql()
        {
            using var context = GetTestContext();
            context.GetMigrator().GetDropSql().Verify();
            context.GetMigrator().GetCreateSql().WriteToConsole();
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _02_CreateSql()
        {
            using var context = GetTestContext();
            context.GetMigrator().GetCreateSql().Verify();
        }

        [TestMethod]
        public void _03_ReadWrite()
        {
            using (var context = GetTestContext())
            {
                var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
                var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

                context.Database.ExecuteSql(dropSql);
                context.Database.ExecuteSql(createSql);

                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().BeNull();

                var defaultEntity = new AuditableEntity { Id = -1, Name = "Minus One" };
                entity = new AuditableEntity { Id = 1, Name = "One" };
                context.__AuditableTestTable__.Add(defaultEntity);
                context.__AuditableTestTable__.Add(entity);

                var nonAuditableEntity = new NonAuditableEntity { Id = 1, Name = "One" };
                context.__NonAuditableTestTable__.Add(nonAuditableEntity);

                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().NotBeNull();

                entity.Id.Should().Be(1);
                entity.Name.Should().Be("One");
                entity.Name = "One Updated";

                context.__AuditableTestTable__.Update(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().NotBeNull();

                entity.Id.Should().Be(1);
                entity.Name.Should().Be("One Updated");

                entity.Name = "One Before Delete";

                context.__AuditableTestTable__.Remove(entity);

                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().BeNull();

                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {-1} AND A.Name = {"Minus One"} AND B.Action = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.Name = {"One"} AND B.Action = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.Name = {"One Updated"} AND B.Action = {"UPDATE"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.Name = {"One Updated"} AND B.Action = {"DELETE"}")
                    .Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void _04_ReadWriteLineItems()
        {
            using (var context = GetTestContext())
            {
                var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
                var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

                context.Database.ExecuteSql(dropSql);
                context.Database.ExecuteSql(createSql);

                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().BeNull();

                var lineItem = context.__AuditableTestLineItemTable__.Find(1);
                lineItem.Should().BeNull();

                entity = new AuditableEntity { Id = 1, Name = "One" };
                lineItem = new TestLineItemEntity { Id = 1, Name = "Line One" };
                entity.LineItems.Add(lineItem);
                
                context.__AuditableTestTable__
                    .Add(entity);

                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Include(e => e.LineItems).SingleOrDefault(e => e.Id == 1);
                entity.Should().NotBeNull();

                var lineItem = context.__AuditableTestLineItemTable__.Find(1);
                lineItem.Should().NotBeNull();
                lineItem.AuditableEntityId.Should().Be(1);
                lineItem.Name.Should().Be("Line One");

                lineItem = entity.LineItems.SingleOrDefault();
                lineItem.AuditableEntityId.Should().Be(1);
                lineItem.Name.Should().Be("Line One");

                lineItem.Name = "Line One Update";

                //context.__AuditableTestTable__.Update(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var lineItem = context.__AuditableTestLineItemTable__.Find(1);
                lineItem.Should().NotBeNull();
                lineItem.AuditableEntityId.Should().Be(1);
                lineItem.Name.Should().Be("Line One Update");
                lineItem.Name = "Line One Update 2";

                context.__AuditableTestLineItemTable__.Update(lineItem);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Include(e => e.LineItems).SingleOrDefault(e => e.Id == 1);
                entity.Should().NotBeNull();
                entity.LineItems.Should().HaveCount(1);

                var lineItem = entity.LineItems.SingleOrDefault();
                lineItem.AuditableEntityId.Should().Be(1);
                lineItem.Name.Should().Be("Line One Update 2");

                entity.LineItems.Remove(lineItem);

                //context.__AuditableTestTable__.Update(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Include(e => e.LineItems).SingleOrDefault(e => e.Id == 1);
                entity.Should().NotBeNull();
                entity.LineItems.Should().HaveCount(0);

                var lineItem2 = new TestLineItemEntity() { Id = 2, Name = "Line Two" };
                context.__AuditableTestLineItemTable__.Add(lineItem2);

                entity.LineItems.Add(lineItem2);

                //context.__AuditableTestTable__.Update(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Include(e => e.LineItems).SingleOrDefault(e => e.Id == 1);
                entity.Should().NotBeNull();
                entity.LineItems.Should().HaveCount(1);

                context.__AuditableTestTable__.Remove(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.Name = {"One"} AND B.Action = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.Name = {"One"} AND B.Action = {"UPDATE"}")
                    .Should().HaveCount(0);

                context.__AuditableTestTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.Name = {"One"} AND B.Action = {"DELETE"}")
                    .Should().HaveCount(1);

                context.__AuditableTestLineItemTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestLineItemTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.AuditableEntityId = {1} AND B.Action = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestLineItemTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestLineItemTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.AuditableEntityId = {1} AND B.Action = {"UPDATE"}")
                    .Should().HaveCount(2);

                context.__AuditableTestLineItemTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestLineItemTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {1} AND A.AuditableEntityId = {1} AND B.Action = {"DELETE"}")
                    .Should().HaveCount(1);

                context.__AuditableTestLineItemTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestLineItemTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {2} AND A.AuditableEntityId = {1} AND B.Action = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestLineItemTable__
                    .FromSql($"SELECT A.* FROM [__AuditableTestLineItemTable___Audit] A LEFT JOIN [Audit] B ON A.AuditId = B.Id WHERE A.Id = {2} AND A.AuditableEntityId = {1} AND B.Action = {"DELETE"}")
                    .Should().HaveCount(1);
            }
        }
    }
}
