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

        public static string ToUTF8String(this byte[] input)
            => Encoding.UTF8.GetString(input);

        public static byte[] ToByteArrayAsBase64(this string input)
            => Convert.FromBase64String(input);

        public static string ToBase64String(this byte[] input)
            => Convert.ToBase64String(input);

        public static byte[] ToSHA256(this byte[] input)
        {
            using var hasher = SHA256.Create();
            return hasher.ComputeHash(input);
        }

        public static byte[] ToByteArrayAsHexString(this string input)
        {
            return input
                .ToEqualLengthSubstrings(2)
                .Select(s => Convert.ToByte(s, 16))
                .ToArray();
        }

        public static IEnumerable<string> ToEqualLengthSubstrings(this string input, int substringLength)
        {
            List<string> output = new List<string>();
            int resultSize = input.Length / substringLength;
            for (int i = 0; i < resultSize; i++)
                output.Add(input.Substring(i * substringLength, substringLength));
            return output;
        }

        public static string ToHexString(this byte[] input) 
            => input
                .Select(x => x.ToString("x2"))
                .Join();

        public static string Join(this IEnumerable<string> input, string separator = "")
            => string.Join(separator, input);
    }
}
