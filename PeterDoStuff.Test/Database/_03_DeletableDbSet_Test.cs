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
    public class _03_DeletableDbSet_Test
    {
        private class DeletableEntity
        {
            public int Id { get; set; }
            
            [MaxLength(100)]
            public string Name { get; set; }
        }

        private class DeletableContext : DbContext
        {
            public DeletableContext(DbContextOptions<DeletableContext> options) : base(options) { }

            [WithDeletedBin]
            public DbSet<DeletableEntity> __DeletableTestTable__ { get; set; }
        }

        private DeletableContext GetTestContext()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var options = new DbContextOptionsBuilder<DeletableContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new DeletableContext(options);
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

                var entity = context.__DeletableTestTable__.Find(1);
                entity.Should().BeNull();

                entity = new DeletableEntity { Id = 1, Name = "One" };
                context.__DeletableTestTable__.Add(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__DeletableTestTable__.Find(1);
                entity.Should().NotBeNull();

                entity.Id.Should().Be(1);
                entity.Name.Should().Be("One");

                context.__DeletableTestTable__.Remove(entity);
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                var entity = context.__DeletableTestTable__.Find(1);
                entity.Should().BeNull();
            }
        }
    }
}
