using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using System.Security.Policy;
using PeterDoStuff.Extensions;
using System.CodeDom;
using System.Diagnostics;
using System.Dynamic;

namespace PeterDoStuff.MudWasmHosted.Client.Extensions
{
    public static class HttpExtensions
    {
        public static HttpRequestBuilder Request(this HttpClient http, HttpMethod method, string url)
        {
            return new HttpRequestBuilder()
            {
                Http = http,
                Method = method,
                Url = url
            };
        }

        private static async Task<(HttpResult<TResponse>, HttpResponseMessage?)> SendAndGetResponse<TResponse>(HttpRequestBuilder @this)
        {
            try
            {
                var url = @this.Url;
                if (@this.Parameters.Any())
                    url += "?" + @this.Parameters.Select(kv => $"{kv.Key}={kv.Value}").Join("&");

                var request = new HttpRequestMessage(@this.Method, url);

                if (@this.Body != null)
                {
                    var jsonContent = JsonSerializer.Serialize(@this.Body);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }
                else if (@this.DynamicBody != null)
                {
                    var jsonContent = JsonSerializer.Serialize(@this.DynamicBody);
                    request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                }

                foreach (var header in @this.Headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
                var response = await @this.Http.SendAsync(request);
                var result = new HttpResult<TResponse>();
                result.Success = response.StatusCode == System.Net.HttpStatusCode.OK;
                return (result, response);
            }
            catch (Exception ex)
            {
                var result = new HttpResult<TResponse>();
                result.Success = false;
                result.Error = ex.Message;
                return (result, null);
            }
        }

        /// <summary>
        /// Send the Http Request and parse the response as type TResponse
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public async static Task<HttpResult<TResponse>> SendAsync<TResponse>(this HttpRequestBuilder @this)
        {
            (HttpResult<TResponse> result, HttpResponseMessage? response) = await SendAndGetResponse<TResponse>(@this);
            if (result.Success)
            {
                if (typeof(string) == typeof(TResponse))
                    result.SetResult(await response.Content.ReadAsStringAsync());
                else
                    result.SetResult(await response.Content.ReadFromJsonAsync<TResponse>());
            }
            else if (result.Error == null && response != null)
            {
                result.Error = response.StatusCode + ": ";
                result.Error += await response.Content.ReadAsStringAsync();
            }
            return result;
        }
    }

    public class HttpRequestBuilder : IDisposable
    {
        public HttpClient Http { get; set; }
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
        public object? Body { get; set; } = null;
        public dynamic? DynamicBody { get; set; } = null;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public HttpRequestBuilder SetParam(string key, string value)
        {
            Parameters[Uri.EscapeDataString(key)] = Uri.EscapeDataString(value??"");
            return this;
        }

        public HttpRequestBuilder SetBody(object body)
        {
            Body = body;
            return this;
        }

        public HttpRequestBuilder SetBodyParam(string key, object value)
        {
            if (DynamicBody == null)
                DynamicBody = new ExpandoObject();
            var kv = DynamicBody as IDictionary<string, object>;
            kv[key] = value;
            return this;
        }

        public HttpRequestBuilder SetHeader(string key, string value)
        {
            Headers[key] = value;
            return this;
        }

        public void Dispose()
        {
            Http.Dispose();
        }
    }

    public class HttpResult<TResponse>
    {
        public bool Success { get; set; } = false;
        public bool Failure => !Success;
        public TResponse? Result { get; private set; }
        public void SetResult(object result)
        {
            Result = (TResponse)result;
        }
        public string? Error { get; set; } = null;
    }
}
