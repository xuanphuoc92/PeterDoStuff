using ApprovalTests.Reporters;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        private class AuditableEntity : TestEntity;

        private class NonAuditableEntity : TestEntity;

        private class AuditableContext : DbContext
        {
            public AuditableContext(DbContextOptions<AuditableContext> options) : base(options) { }

            [Auditable]
            public DbSet<AuditableEntity> __AuditableTestTable__ { get; set; }

            public DbSet<NonAuditableEntity> __NonAuditableTestTable__ { get; set; }

            public override int SaveChanges()
            {
                this.GetAuditor().AuditChanges();

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
                    .FromSql($"SELECT * FROM [__AuditableTestTable___Audit] WHERE Id = {-1} AND Name = {"Minus One"} AND AuditAction = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT * FROM [__AuditableTestTable___Audit] WHERE Id = {1} AND Name = {"One"} AND AuditAction = {"INSERT"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT * FROM [__AuditableTestTable___Audit] WHERE Id = {1} AND Name = {"One Updated"} AND AuditAction = {"UPDATE"}")
                    .Should().HaveCount(1);

                context.__AuditableTestTable__
                    .FromSql($"SELECT * FROM [__AuditableTestTable___Audit] WHERE Id = {1} AND Name = {"One Updated"} AND AuditAction = {"DELETE"}")
                    .Should().HaveCount(1);
            }
        }
    }
}
