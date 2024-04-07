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
    public class _01b_Context_SqlServer_Test : _01_Context_Test
    {
        protected override UserContext GetTestContext()
        {
            var connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new UserContext(options);
        }
    }
}
