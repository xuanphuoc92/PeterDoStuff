using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PeterDoStuff.Identity
{
    public class User
    {
        //public Guid Id { get; set; }
        
        //[Required]
        //[MaxLength(100)]
        //public string Name { get; set; }

        //public List<UserAuth> Auths { get; set; }
    }

    public class UserAuth
    {
        //public Guid Id { get; set; }
        //public Guid UserId { get; set; }

        //[MaxLength(100)]
        //public string? UserName { get; set; }
        //[MaxLength(32)]
        //public byte[]? PasswordHash { get; set; }
        //[MaxLength(16)]
        //public byte[]? PasswordSalt { get; set; }
    }
}
