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
        private static async Task TestCreateTable(MemoryConnection conn)
        {
            // Drop the table first
            await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_];");

            bool tableExists;
            
            // Table dropped, should not exist
            tableExists = await conn.TableExists("_TestTable_");
            tableExists.Should().BeFalse();

            // Table created
            await conn.ExecuteAsync("CREATE TABLE [_TestTable_] ([Id] uniqueidentifier);");

            // Within transaction, table is queried and exists
            tableExists = await conn.TableExists("_TestTable_");
            tableExists.Should().BeTrue();
        }

        [TestMethod]
        public async Task _01_Commit()
        {
            using var db = new MemoryDb();
            
            using (var conn = db.Open())
            {
                await TestCreateTable(conn);
                //The transaction is committed
                conn.Commit();
            }

            using (var conn = db.Open())
            {
                // Outside and in another transaction, the table exist
                bool tableExists = await conn.TableExists("_TestTable_");
                tableExists.Should().BeTrue();
            }
        }

        [TestMethod]
        public async Task _02_Rollback()
        {
            using var db = new MemoryDb();

            using (var conn = db.Open())
            {
                await TestCreateTable(conn);
                // But transaction not committed
                //conn.Commit();
            }

            using (var conn = db.Open())
            {
                // Outside and in another transaction, table not exist
                bool tableExists = await conn.TableExists("_TestTable_");
                tableExists.Should().BeFalse();
            }
        }
    }
}
