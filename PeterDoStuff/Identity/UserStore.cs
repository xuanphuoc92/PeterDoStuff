namespace PeterDoStuff.Identity
{
    public class UserStore
    {
        public List<User> Users { get; set; } = [];

        public static Guid DEFAULT_USER_ID = new Guid("c00b161e-ba81-469e-92e1-07dbd66cd03f");

        public static UserStore Default = new UserStore()
        {
            Users = [
                new User() {
                    Id = UserStore.DEFAULT_USER_ID,
                    Name = "Super Admin",
                    Auths = [
                        new UserAuth() {
                            Id = Guid.NewGuid(),
                            UserId = UserStore.DEFAULT_USER_ID,
                            AuthId = "Admin",
                            Password = new Password("admin")
                        }
                    ]
                }
            ]
        };
    }
}
