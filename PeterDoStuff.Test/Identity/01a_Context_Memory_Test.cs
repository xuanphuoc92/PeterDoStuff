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
    public class _01a_Context_Memory_Test : _01_Context_Test
    {
        protected override UserContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new UserContext(options);
        }
    }
}
