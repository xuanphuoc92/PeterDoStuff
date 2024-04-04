using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Extensions;
using PeterDoStuff.Identity;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _06_EfCoreExtensions_Test
    {
        private UserContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new UserContext(options);
        }

        [TestMethod]
        public void _01_GetMigrateSql()
        {
            using var context = GetTestContext();
            context.GetMigrateSql().WriteToConsole();
        }
    }
}
