using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Identity
{
    [TestClass]
    public class _02_Service_Test
    {
        private UserService GetTestService()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var context = new UserContext(options);
            return new UserService(context);
        }

        [TestMethod]
        public void _01_RegisterAuthenticate()
        {
            using var service = GetTestService();
            
            // Register 1st time
            service.Register("NewUser", "P@ssw0rd").Should().BeTrue();

            // Register 2nd time
            service.Register("NewUser", "P@ssw0rd").Should().BeFalse();

            // Authenticate
            service.Authenticate("NonExistUser", "P@ssw0rd").Should().BeFalse();
            service.Authenticate("NewUser", "WrongP@ssw0rd").Should().BeFalse();
            service.Authenticate("NewUser", "P@ssw0rd").Should().BeTrue();
        }
    }
}
