using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Extensions;
using PeterDoStuff.Identity;
using PeterDoStuff.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Test.Identity
{
    public abstract class _01_Context_Test
    {
        protected abstract UserContext GetTestContext();

        [TestMethod]
        public void _01_Setup()
        {
            using (var context = GetTestContext())
            {
                Setup(context);

                context.Model.FindEntityType(typeof(User)).Should().NotBeNull();
                context.Model.FindEntityType(typeof(UserAuth)).Should().NotBeNull();
            }
        }

        private static void Setup(UserContext context)
        {
            var dropSql = FormattableStringFactory.Create(context.GetMigrator().GetDropSql());
            var createSql = FormattableStringFactory.Create(context.GetMigrator().GetCreateSql());

            context.Database.ExecuteSql(dropSql);
            context.Database.ExecuteSql(createSql);
        }

        [TestMethod]
        public void _02_Add()
        {
            Guid userId, userAuthId;

            using (var context = GetTestContext())
            {
                Setup(context);

                User user = new User() { Name = "Test" };
                context.Users.Add(user);
                UserAuth userAuth = new UserAuth();
                context.UserAuths.Add(userAuth);
                context.SaveChanges();

                userId = user.Id;
                userAuthId = userAuth.Id;

                context.Users.Find(userId).Should().NotBeNull();
                context.UserAuths.Find(userAuthId).Should().NotBeNull();

                context.Users.Find(userAuthId).Should().BeNull();
                context.UserAuths.Find(userId).Should().BeNull();

                user = context.Users.Find(userId);
                user.Id.ToString().WriteToConsole();
                user.Id.Should().NotBe(new Guid());
                user.Id.Should().Be(userId);
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
            
                userId.ToString().WriteToConsole("userId");
                userAuthId.ToString().WriteToConsole("userAuthId");
            
                user = context.Users.Find(userId);
                
                user.Should().NotBeNull();
                user.Id.Should().Be(userId);
                //user.Auths.Should().BeNullOrEmpty();

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
