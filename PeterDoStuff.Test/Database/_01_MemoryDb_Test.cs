using FluentAssertions;
using PeterDoStuff.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Database
{
    [TestClass]
    public class _01_MemoryDb_Test
    {
        [TestMethod]
        public async Task _01_CreateTable()
        {
            using var db = new MemoryDb();
            using var conn = db.Open();
            await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_];");
            bool tableExists;
            tableExists = await conn.TableExists("_TestTable_");
            tableExists.Should().BeFalse();
            await conn.ExecuteAsync("CREATE TABLE [_TestTable_] ([Id] uniqueidentifier);");
            tableExists = await conn.TableExists("_TestTable_");
            tableExists.Should().BeTrue();
            conn.Commit();
        }
    }
}
