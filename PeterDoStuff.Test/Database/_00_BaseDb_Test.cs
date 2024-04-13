using FluentAssertions;
using PeterDoStuff.Database;
using PeterDoStuff.Test.Extensions;
using PeterDoStuff.Tools;

namespace PeterDoStuff.Test.Database
{
    public abstract class _00_BaseDb_Test
    {
        protected abstract BaseDb GetDb();

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
            using var db = GetDb();

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
            using var db = GetDb();

            using (var conn = db.Open())
            {
                await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_];");
                conn.Commit();
            }

            using (var conn = db.Open())
            {
                await TestCreateTable(conn);
                // But transaction not committed
                //conn.Commit();
            }

            using (var conn = db.Open())
            {
                using (var nestedConn = db.Open())
                {
                    // Outside and in another transaction, table not exist
                    bool tableExists = await nestedConn.TableExists("_TestTable_");
                    tableExists.Should().BeFalse();
                }
            }
        }

        [TestMethod]
        public async Task _03_Query()
        {
            using var db = GetDb();

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
            using var db = GetDb();

            using var conn = db.Open();
            var rowCount = await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_]");
            rowCount.Should().Be(db is MemoryDb ? 0 : -1);

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
            rowCount.Should().Be(db is MemoryDb ? 3 : -1);

            // Execute behaviour:
            // If SQL is pure Query, return -1
            // If SQL is mixed and ended with Query, return **SUM** of Execute.
            // Execute over Table in SQLite = rows cached in table
            // Execute over Table in SQL Server = -1
        }

        [TestMethod]
        public async Task _05_ExecuteOrQuery()
        {
            using var db = GetDb();

            var queryEditor = new QueryEditor(db);

            var output = await queryEditor.ExecuteOrQueryAsync("DROP TABLE IF EXISTS [_TestTable_]");
            output.Execute.Should().Be(db is MemoryDb ? 0 : -1);

            output = await queryEditor.ExecuteOrQueryAsync(SqlCommand.SAMPLE_TEST_SQL);
            output.DynamicQuery().Should().HaveCount(3);

            output = await queryEditor.ExecuteOrQueryAsync("DELETE FROM [_TestTable_];");            
            output.Execute.Should().Be(3);

            output = await queryEditor.ExecuteOrQueryAsync("SELECT * FROM [_TestTable_];");            
            output.DynamicQuery().Should().HaveCount(0);

            output = await queryEditor.ExecuteOrQueryAsync("DROP TABLE IF EXISTS [_TestTable_]");
            output.Execute.Should().Be(db is MemoryDb ? 3 : -1);
        }

        private async Task Setup(BaseDb db, string sql)
        {
            using (var conn = db.Open())
            {
                await conn.ExecuteAsync(sql);
                conn.Commit();
            }
        }

        [TestMethod]
        public async Task _06_Nested_InRollback_OutRollback()
        {
            using var db = GetDb();

            await Setup(db, SqlCommand.SAMPLE_TEST_SQL);

            using (var outerConn = db.Open())
            {
                await NegativeVerify(outerConn);
            }

            using (var outerConn = db.Open())
            {
                using (var innerConn = db.Open())
                {
                    await NegativeVerify(innerConn);
                    await ExecuteChange(innerConn);
                    await PositiveVerify(innerConn);
                }
                await PositiveVerify(outerConn);
            }

            using (var outerConn = db.Open())
            {
                await NegativeVerify(outerConn);
            }
        }


        [TestMethod]
        public async Task _07_Nested_InCommit_OutRollback()
        {
            using var db = GetDb();

            await Setup(db, SqlCommand.SAMPLE_TEST_SQL);

            using (var outerConn = db.Open())
            {
                await NegativeVerify(outerConn);
            }

            using (var outerConn = db.Open())
            {
                using (var innerConn = db.Open())
                {
                    await NegativeVerify(innerConn);
                    await ExecuteChange(innerConn);
                    await PositiveVerify(innerConn);
                    innerConn.Commit();
                }
                await PositiveVerify(outerConn);
            }

            using (var outerConn = db.Open())
            {
                await NegativeVerify(outerConn);
            }
        }

        [TestMethod]
        public async Task _08_Nested_InRollback_OutCommit()
        {
            using var db = GetDb();

            await Setup(db, SqlCommand.SAMPLE_TEST_SQL);

            using (var outerConn = db.Open())
            {
                await NegativeVerify(outerConn);
            }

            using (var outerConn = db.Open())
            {
                using (var innerConn = db.Open())
                {
                    await NegativeVerify(innerConn);
                    await ExecuteChange(innerConn);
                    await PositiveVerify(innerConn);
                }
                await PositiveVerify(outerConn);
                Action commitAction = () => outerConn.Commit();
                var ex = commitAction.Should().Throw<Exception>().Subject.Single();
                ex.WriteToConsole();
            }
        }

        [TestMethod]
        public async Task _09_Nested_InCommit_OutCommit()
        {
            using var db = GetDb();

            await Setup(db, SqlCommand.SAMPLE_TEST_SQL);

            using (var outerConn = db.Open())
            {
                await NegativeVerify(outerConn);
            }

            using (var outerConn = db.Open())
            {
                using (var innerConn = db.Open())
                {
                    await NegativeVerify(innerConn);
                    await ExecuteChange(innerConn);
                    await PositiveVerify(innerConn);
                    innerConn.Commit();
                }
                await PositiveVerify(outerConn);
                outerConn.Commit();
            }

            using (var outerConn = db.Open())
            {
                await PositiveVerify(outerConn);
            }
        }

        private async Task ExecuteChange(BaseConnection conn)
        {
            int execute;
            execute = await conn.ExecuteAsync("INSERT INTO [_TestTable_] (Number, Name) VALUES (4, 'David')");
            execute.Should().Be(1);
            execute = await conn.ExecuteAsync("UPDATE [_TestTable_] SET Name = 'Alex' WHERE Number = 1");
            execute.Should().Be(1);
            execute = await conn.ExecuteAsync("DELETE FROM [_TestTable_] WHERE Number = 2");
            execute.Should().Be(1);
        }

        private class TestTableRow
        {
            public int Number;
            public string Name;
        }

        private async Task PositiveVerify(BaseConnection conn)
        {
            var query = await conn.QueryAsync<TestTableRow>("SELECT * FROM [_TestTable_]");
            query.Should().HaveCount(3);
            query.Single(d => d.Number == 1).Name.Should().Be("Alex");
            query.Single(d => d.Number == 3).Name.Should().Be("Carol");
            query.Single(d => d.Number == 4).Name.Should().Be("David");
        }

        private async Task NegativeVerify(BaseConnection conn)
        {
            var query = await conn.QueryAsync<TestTableRow>("SELECT * FROM [_TestTable_]");
            query.Should().HaveCount(3);
            query.Single(d => d.Number == 1).Name.Should().Be("Alice");
            query.Single(d => d.Number == 2).Name.Should().Be("Bob");
            query.Single(d => d.Number == 3).Name.Should().Be("Carol");
        }

        [TestMethod]
        public async Task _10_NestedRollback()
        {
            using var db = GetDb();
            using (var conn = db.Open())
            {
                await Setup_NestedTransactions(conn);
                conn.Commit();
            }

            using (var conn1 = db.Open())
            {
                await Verify_NestedTransactions(conn1, 0);
                await Update_NestedTransactions(conn1);
                using (var conn2 = db.Open())
                {
                    await Verify_NestedTransactions(conn2, 1);
                    await Update_NestedTransactions(conn2);
                    using (var conn3 = db.Open())
                    {
                        await Verify_NestedTransactions(conn3, 2);
                        await Update_NestedTransactions(conn3);
                    }
                    await Verify_NestedTransactions(conn2, 3);
                    await Update_NestedTransactions(conn2);
                }
                await Verify_NestedTransactions(conn1, 4);
                await Update_NestedTransactions(conn1);
            }

            using (var conn1 = db.Open())
            {
                await Verify_NestedTransactions(conn1, 0);
            }
        }

        [TestMethod]
        public async Task _11_NestedCommits()
        {
            using var db = GetDb();
            using (var conn = db.Open())
            {
                await Setup_NestedTransactions(conn);
                conn.Commit();
            }

            using (var conn1 = db.Open())
            {
                await Verify_NestedTransactions(conn1, 0);
                await Update_NestedTransactions(conn1);
                using (var conn2 = db.Open())
                {
                    await Verify_NestedTransactions(conn2, 1);
                    await Update_NestedTransactions(conn2);
                    using (var conn3 = db.Open())
                    {
                        await Verify_NestedTransactions(conn3, 2);
                        await Update_NestedTransactions(conn3);
                        conn3.Commit();
                    }
                    await Verify_NestedTransactions(conn2, 3);
                    await Update_NestedTransactions(conn2);
                    conn2.Commit();
                }
                await Verify_NestedTransactions(conn1, 4);
                await Update_NestedTransactions(conn1);
                conn1.Commit();
            }

            using (var conn1 = db.Open())
            {
                await Verify_NestedTransactions(conn1, 5);
            }
        }

        [TestMethod]
        public async Task _12_Nested_InnerRollback_Exist()
        {
            using var db = GetDb();
            using (var outer = db.Open())
            {
                await Setup_NestedTransactions(outer);
                outer.Commit();
            }
            
            using (var outer = db.Open())
            {
                using (var inner = db.Open())
                {
                    await Verify_NestedTransactions(inner, 0);
                    await Update_NestedTransactions(inner);
                    inner.Commit();
                }

                using (var inner = db.Open())
                {
                    await Verify_NestedTransactions(inner, 1);
                    await Update_NestedTransactions(inner);
                    //inner.Commit();
                }

                using (var inner = db.Open())
                {
                    await Verify_NestedTransactions(inner, 2);
                    await Update_NestedTransactions(inner);
                    inner.Commit();
                }

                Action commitAction = () => outer.Commit();
                var exception = commitAction.Should().Throw<Exception>().Subject.Single();
                exception.WriteToConsole();
            }

            using (var outer = db.Open())
            {
                await Verify_NestedTransactions(outer, 0);
            }
        }

        private static async Task Setup_NestedTransactions(BaseConnection conn)
        {
            await conn.ExecuteAsync("DROP TABLE IF EXISTS [_TestTable_];");
            await conn.ExecuteAsync("CREATE TABLE [_TestTable_] (Id int);");
            await conn.ExecuteAsync("INSERT INTO [_TestTable_] (Id) VALUES (0);");
        }

        private static async Task Verify_NestedTransactions(BaseConnection conn, int verifiedValue)
        {
            int value = (await conn.QueryAsync("SELECT Id FROM [_TestTable_];")).Single().Id;
            value.Should().Be(verifiedValue);
        }

        private static async Task Update_NestedTransactions(BaseConnection conn)
        {
            await conn.ExecuteAsync("UPDATE [_TestTable_] SET Id = Id + 1;");
        }
    }
}
