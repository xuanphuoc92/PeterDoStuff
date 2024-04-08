using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace PeterDoStuff.Test.Database
{
    [TestClass]
    public class _03_AuditableDbSet_Test
    {
        private class AuditableEntity
        {
            public int Id { get; set; }
            
            [MaxLength(100)]
            public string Name { get; set; }
        }

        private class AuditableContext : DbContext
        {
            public AuditableContext(DbContextOptions<AuditableContext> options) : base(options) { }

            [Auditable]
            public DbSet<AuditableEntity> __AuditableTestTable__ { get; set; }
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
        public void _01_Migrator()
        {
            using var context = GetTestContext();
            context.GetMigrator().GetDropSql().WriteToConsole();
            context.GetMigrator().GetCreateSql().WriteToConsole();
        }

        [TestMethod]
        public void _02_ReadWrite()
        {
            using (var context = GetTestContext())
            {
                var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
                var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

                context.Database.ExecuteSql(dropSql);
                context.Database.ExecuteSql(createSql);

                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().BeNull();

                entity = new AuditableEntity { Id = 1, Name = "One" };
                context.__AuditableTestTable__.Add(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().NotBeNull();

                entity.Id.Should().Be(1);
                entity.Name.Should().Be("One");
                entity.Name = "Two";

                context.__AuditableTestTable__.Update(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().NotBeNull();

                entity.Id.Should().Be(1);
                entity.Name.Should().Be("Two");

                context.__AuditableTestTable__.Remove(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__AuditableTestTable__.Find(1);
                entity.Should().BeNull();
            }
        }
    }
}
