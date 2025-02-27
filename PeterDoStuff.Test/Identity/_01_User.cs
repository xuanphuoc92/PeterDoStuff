using FluentAssertions;
using PeterDoStuff.Identity;

namespace PeterDoStuff.Test.Identity
{
    [TestClass]
    public class _01_User
    {
        [TestMethod]
        public void _01_DefaultUserStore()
        {
            UserStore userStore = UserStore.Default;
            userStore.Users.Should().HaveCount(1);
        }
    }
}
