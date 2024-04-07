using Microsoft.EntityFrameworkCore;
using PeterDoStuff.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PeterDoStuff.Identity
{
    public class User
    {
        public Guid Id { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<UserAuth>? Auths { get; set; }
    }

    public class UserAuth
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        [MaxLength(100)]
        public string? UserName { get; set; }
        [MaxLength(32)]
        public byte[]? PasswordHash { get; set; }
        [MaxLength(16)]
        public byte[]? PasswordSalt { get; set; }
    }

    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserAuth> UserAuths => Set<UserAuth>();
    }

    public class UserService : IDisposable
    {
        private readonly UserContext _context;
        public UserService (UserContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task<bool> Register(string userName, string password)
        {
            userName = userName.ToLower();
            var auth = await _context.UserAuths
                .SingleOrDefaultAsync(ua => ua.UserName != null && ua.UserName.ToLower() == userName);
            if (auth != null)
                return false;

            var user = new User();
            user.Name = userName;
            user.Auths = new List<UserAuth>();
            
            auth = new UserAuth();
            auth.UserName = userName;
            auth.PasswordSalt = SecurityExtensions.GenerateSalt();
            auth.PasswordHash = password.ToByteArray().HashArgon2id(auth.PasswordSalt);

            user.Auths.Add(auth);

            await _context.Users.AddAsync(user);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> Authenticate(string userName, string password)
        {
            userName = userName.ToLower();
            var auth = await _context.UserAuths
                .SingleOrDefaultAsync(ua => ua.UserName != null && ua.UserName.ToLower() == userName);

            if (auth == null) return false;

            var passwordHash = password.ToByteArray().HashArgon2id(auth.PasswordSalt);

            return passwordHash.SequenceEqual(auth.PasswordHash);
        }
    }
}
