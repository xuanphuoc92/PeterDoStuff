using System.Text.Json;

namespace PeterDoStuff.Extensions
{
    public static class JsonExtensions
    {
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
