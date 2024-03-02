using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PeterDoStuff.Extensions
{
    /// <summary>
    /// Extensions for Strings and Byte Arrays
    /// </summary>
    public static class StringAndBytesExtensions
    {
        /// <summary>
        /// Convert (UTF8) string to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this string input) 
            => Encoding.UTF8.GetBytes(input);

        /// <summary>
        /// Convert byte array to (UTF8) string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToUTF8String(this byte[] input)
            => Encoding.UTF8.GetString(input);

        /// <summary>
        /// Convert base64 string (e.g. image source in HTML) to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToByteArrayAsBase64(this string input)
            => Convert.FromBase64String(input);

        /// <summary>
        /// Convert byte array to base64 string (e.g. image source in HTML)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] input)
            => Convert.ToBase64String(input);

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
        /// Convert hex string to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToByteArrayAsHexString(this string input)
        {
            return input
                .ToEqualLengthSubstrings(2)
                .Select(s => Convert.ToByte(s, 16))
                .ToArray();
        }

        /// <summary>
        /// Split a string to substrings of n length
        /// </summary>
        /// <param name="input"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static IEnumerable<string> ToEqualLengthSubstrings(this string input, int n)
        {
            List<string> output = new List<string>();
            int resultSize = input.Length / n;
            for (int i = 0; i < resultSize; i++)
                output.Add(input.Substring(i * n, n));
            return output;
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] input) 
            => input
                .Select(x => x.ToString("x2"))
                .Join();

        /// <summary>
        /// Combine strings into one string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string Join(this IEnumerable<string> input, string separator = "")
            => string.Join(separator, input);
    }
}
