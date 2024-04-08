using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Attributes;
using PeterDoStuff.Extensions;
using PeterDoStuff.Test.Extensions;
using System.ComponentModel.DataAnnotations;

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
    }
}
