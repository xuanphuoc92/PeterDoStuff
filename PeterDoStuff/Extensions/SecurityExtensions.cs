using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Extensions
{
    /// <summary>
    /// Extensions for Security features such as Hashing and Encryption
    /// </summary>
    public static class SecurityExtensions
    {
        /// <summary>
        /// Compute the SHA256 Hash Value of a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToSHA256(this byte[] input)
        {
            using var hasher = SHA256.Create();
            return hasher.ComputeHash(input);
        }

        /// <summary>
        /// Compute the SHA512 Hash Value of a byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToSHA512(this byte[] input)
        {
            using var hasher = SHA512.Create();
            return hasher.ComputeHash(input);
        }
    }
}
