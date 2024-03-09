using System.Text;

namespace PeterDoStuff.Extensions
{
    /// <summary>
    /// Extensions for Strings and Byte Arrays
    /// </summary>
    public static class StringAndBytesExtensions
    {
        public static bool IsNullOrEmpty(this string input)
            => string.IsNullOrEmpty(input);

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
        public static byte[] ToByteArrayAsBase64String(this string input)
            => Convert.FromBase64String(input);

        /// <summary>
        /// Convert byte array to base64 string (e.g. image source in HTML)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToBase64String(this byte[] input)
            => Convert.ToBase64String(input);

        /// <summary>
        /// Convert hex string to byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] ToByteArrayAsHexString(this string input)
            => Convert.FromHexString(input);

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] input) 
            => Convert.ToHexString(input);

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
