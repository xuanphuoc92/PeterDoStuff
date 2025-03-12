using FluentAssertions;
using PeterDoStuff.Database;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Database
{
    [TestClass]
    public class _02_SqlServerDb_Test : _00_BaseDb_Test
    {
        protected override BaseDb GetDb()
        {
            return new SqlServerDb(
                connString: "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=PeterDoStuffDb;Integrated Security=True;Connect Timeout=10;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
                );
        }

        [TestMethod]
        public void _99_InvalidConnString()
        {
            using var db = new SqlServerDb(
                connString: "InvalidConnString");
            Action tryOpenConnection = () => { using var conn = db.Open(); };
            var exception = tryOpenConnection.Should().Throw<Exception>().Subject.Single();
            exception.Message.WriteToConsole();
        }
    }
}
