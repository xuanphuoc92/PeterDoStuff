using System.Text;
using System.Text.Json;

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

        /// <summary>
        /// Combine strings into one string separated with new lins
        /// </summary>
        /// <param name="input"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinLines(this IEnumerable<string> input)
            => input.Join(Environment.NewLine);

        /// <summary>
        /// Serialize an object into Json String
        /// </summary>
        /// <param name="this"></param>
        /// <returns></returns>
        public static string ToJson(this object @this, bool beautify = false)
        {
            string jsonString = JsonSerializer.Serialize(@this);

            if (beautify == false)
                return jsonString;

            using var jsonDocument = JsonDocument.Parse(jsonString);
            return JsonSerializer.Serialize(
                    value: jsonDocument.RootElement,
                    options: new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );
        }

        /// <summary>
        /// Deserialize a Json String into an object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string @this)
            => JsonSerializer.Deserialize<T>(@this);

    }
}
