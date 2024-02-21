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
        private static async Task TestCreateTable(BaseConnection conn)
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

        [TestMethod]
        public async Task _03_Query()
        {
            using var db = new MemoryDb();

            using var conn = db.Open();
            var result = await conn.QueryAsync("DROP TABLE IF EXISTS [_TestTable_]");
            result.Should().BeEmpty();

            // Mixed, ended with Query
            result = await conn.QueryAsync(SqlCommand.SAMPLE_TEST_SQL);
            result.Should().HaveCount(3);

            result = await conn.QueryAsync("SELECT * FROM [_TestTable_];");
            result.Should().HaveCount(3);

            // Mixed, ended with Execute
            result = await conn.QueryAsync(@"SELECT * FROM [_TestTable_];
SELECT [Number] FROM [_TestTable_];
DELETE FROM [_TestTable_];");
            result.Should().HaveCount(3);

            result = await conn.QueryAsync("SELECT * FROM [_TestTable_];");
            result.Should().BeEmpty();

            result = await conn.QueryAsync("DROP TABLE IF EXISTS [_TestTable_]");
            result.Should().BeEmpty();

            // Query behaviour:
            // If SQL is pure Execute, return Empty.
            // If SQL is mixed and ended with Execute, return **LAST** Query.
        }

        [TestMethod]
        public async Task _04_Execute()
        {
            using var db = new MemoryDb();

            using var conn = db.Open();
            var rowCount = await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_]");
            rowCount.Should().Be(0);

            // Mixed, ended with Query
            rowCount = await conn.ExecuteAsync(SqlCommand.SAMPLE_TEST_SQL);
            rowCount.Should().Be(3);

            rowCount = await conn.ExecuteAsync("SELECT * FROM [_TestTable_];");
            rowCount.Should().Be(-1);

            // Mixed, ended with Execute
            rowCount = await conn.ExecuteAsync(@"SELECT * FROM [_TestTable_];
SELECT [Number] FROM [_TestTable_];
DELETE FROM [_TestTable_];");
            rowCount.Should().Be(3);

            rowCount = await conn.ExecuteAsync("SELECT * FROM [_TestTable_];");
            rowCount.Should().Be(-1);

            rowCount = await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_]");
            rowCount.Should().Be(3);

            // Execute behaviour:
            // If SQL is pure Query, return -1
            // If SQL is mixed and ended with Query, return **SUM** of Execute.
        }

        [TestMethod]
        public async Task _05_ExecuteOrQuery()
        {
            using var db = new MemoryDb();

            var output = await db.ExecuteOrQueryAsync("DROP TABLE IF EXISTS [_TestTable_]");
            output.Execute.Should().Be(0);

            output = await db.ExecuteOrQueryAsync(SqlCommand.SAMPLE_TEST_SQL);
            output.Query.Should().HaveCount(3);

            output = await db.ExecuteOrQueryAsync("DELETE FROM [_TestTable_];");            
            output.Execute.Should().Be(3);

            output = await db.ExecuteOrQueryAsync("SELECT * FROM [_TestTable_];");            
            output.Query.Should().HaveCount(0);

            output = await db.ExecuteOrQueryAsync("DROP TABLE IF EXISTS [_TestTable_]");
            output.Execute.Should().Be(3);
        }
    }
}
