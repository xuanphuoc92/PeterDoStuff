using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Extensions
{
    public static class StringAndBytesExtensions
    {
        public static byte[] ToByteArray(this string input) 
            => Encoding.UTF8.GetBytes(input);

        public static byte[] ToSHA256(this byte[] input)
        {
            using var hasher = SHA256.Create();
            return hasher.ComputeHash(input);
        }

        public static string ToHexString(this byte[] input) 
            => input
                .Select(x => x.ToString("x2"))
                .Join();

        public static string Join(this IEnumerable<string> input, string separator = "")
            => string.Join(separator, input);
    }
}
