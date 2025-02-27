using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Extensions;
using System.ComponentModel.DataAnnotations;

namespace PeterDoStuff.Identity
{
    public class User
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public List<UserAuth> Auths { get; set; } = [];
    }

    public class UserAuth
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public string AuthType { get; set; } = "NORMAL";
        [Required]
        [MaxLength(100)]
        public string AuthId { get; set; }

        public Password? Password { get; set; }
    }

    public class Password
    {
        public Password(string rawPassword)
        {
            Salt = SecurityExtensions.GenerateSalt();
            Hash = rawPassword.ToByteArray().HashArgon2id(Salt);
        }

        //public bool Validate(string rawPassword)
        //{
        //    var hash = rawPassword.ToByteArray().HashArgon2id(Salt);
        //    return Hash?.SequenceEqual(hash) == true;
        //}

        [MaxLength(32)]
        public byte[] Hash { get; set; }
        [MaxLength(16)]
        public byte[] Salt { get; set; }
    }
}
