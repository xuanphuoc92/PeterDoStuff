using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Identity;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Identity
{
    [TestClass]
    public class _01_Context_Test
    {
        private UserContext GetTestContext()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            return new UserContext(options);
        }

        [TestMethod]
        public void _01_Setup()
        {
            using (var context = GetTestContext())
            {
                context.Database.EnsureCreated().Should().BeTrue();
                context.Model.FindEntityType(typeof(User)).Should().NotBeNull();
                context.Model.FindEntityType(typeof(UserAuth)).Should().NotBeNull();
            }
        }

        [TestMethod]
        public void _02_Add()
        {
            using (var context = GetTestContext())
            {
                context.Users.Add(new User() { Name = "Test" });
                context.UserAuths.Add(new UserAuth());
                context.SaveChanges();
            }

            using (var context = GetTestContext())
            {
                context.Users.Count().Should().Be(1);
                context.UserAuths.Count().Should().Be(1);
                
                var user = context.Users.Single();
                user.Id.ToString().WriteToConsole();
                user.Id.Should().NotBe(new Guid());
            }
        }

        [TestMethod]
        public void _03_AddRelation()
        {
            Guid userId, userAuthId;

            using (var context = GetTestContext())
            {   
                User user = new User() { Name = "Test" };
                user.Auths = [new UserAuth()];
                context.Users.Add(user);
                context.SaveChanges();

                userId = user.Id;
                userAuthId = user.Auths.Single().Id;
            }

            userId.ToString().WriteToConsole("userId");
            userAuthId.ToString().WriteToConsole("userAuthId");

            using (var context = GetTestContext())
            {
                User? user = context.Users.Find(userId);
                
                user.Should().NotBeNull();
                user.Id.Should().Be(userId);
                user.Auths.Should().BeNullOrEmpty();

                user = context.Users
                    .Include(u => u.Auths)
                    .AsNoTracking()
                    .SingleOrDefault(u => u.Id == userId);

                user.Should().NotBeNull();
                user.Id.Should().Be(userId);
                user.Auths.Should().NotBeNullOrEmpty();
                user.Auths.Single().Id.Should().Be(userAuthId);
                user.Auths.Single().UserId.Should().Be(userId);

            }
        }
    }
}
