﻿using ApprovalTests.Reporters;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Extensions;
using PeterDoStuff.Identity;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Extensions_Test
{
    [TestClass]
    public class _06_MigratorExtensions_Test
    {
        private class TestContext : DbContext
        {
            public TestContext(DbContextOptions<TestContext> options) : base(options) { }
            public DbSet<TestEntity> __TestTable__ => Set<TestEntity>();
        }

        private class TestEntity
        {
            public Guid Id { get; set; }
            
            [MaxLength(100)]
            public string? Name { get; set; }
            public string? Description { get; set; }

            [MaxLength(32)]
            public byte[]? StandardHash { get; set; }

            public byte[]? BigHash { get; set; }

            public decimal DefaultDecimal { get; set; }

            [DecimalPrecisionScale(20, 8)]            
            public decimal CustomDecimal { get; set; }

            public int Number { get; set; }

            public float Longitude { get; set; }
            public double Latitude { get; set; }
        }

        private TestContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<TestContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new TestContext(options);
        }

        [TestMethod]
        [UseReporter(typeof(DiffReporter))]
        public void _01_Sql()
        {
            using var context = GetTestContext();
            context.GetMigrator().Sql().Verify();
        }
    }
}