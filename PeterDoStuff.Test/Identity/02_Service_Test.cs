using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Identity;
using System.Runtime.CompilerServices;
using PeterDoStuff.Extensions;

namespace PeterDoStuff.Test.Identity
{
    [TestClass]
    public class _02_Service_Test
    {
        private UserService GetTestService()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseSqlite("Data Source=:memory:")
                .Options;

            var context = new UserContext(options);

            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
            var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

            context.Database.ExecuteSql(dropSql);
            context.Database.ExecuteSql(createSql);

            return new UserService(context);
        }

        [TestMethod]
        public async Task _01_RegisterAuthenticate()
        {
            using var service = GetTestService();
            
            // Register 1st time
            (await service.Register("NewUser", "P@ssw0rd")).Should().BeTrue();

            // Register 2nd time
            (await service.Register("NewUser", "P@ssw0rd")).Should().BeFalse();

            // Authenticate
            (await service.Authenticate("NonExistUser", "P@ssw0rd")).Should().BeFalse();
            (await service.Authenticate("NewUser", "WrongP@ssw0rd")).Should().BeFalse();
            (await service.Authenticate("NewUser", "P@ssw0rd")).Should().BeTrue();
        }
    }
}
