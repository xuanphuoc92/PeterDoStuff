using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Text;
using System.Net.Http.Json;
using System.Security.Policy;
using PeterDoStuff.Extensions;
using System.CodeDom;

namespace PeterDoStuff.MudWasmHosted.Client.Extensions
{
    public static class HttpExtensions
    {
        public static HttpRequest Request(this HttpClient http, HttpMethod method, string url)
        {
            return new HttpRequest()
            {
                Http = http,
                Method = method,
                Url = url
            };
        }

        private static async Task<(HttpResult<TResponse>, HttpResponseMessage?)> SendAndGetResponse<TResponse>(HttpRequest @this)
        {
            try
            {
                var url = @this.Url;
                if (@this.Parameters.Any())
                    url += "?" + @this.Parameters.Select(kv => $"{kv.Key}={kv.Value}").Join("&");
                var request = new HttpRequestMessage(@this.Method, url);
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

        public async static Task<HttpResult<TResponse>> SendAsync<TResponse>(this HttpRequest @this)
        {
            (HttpResult<TResponse> result, HttpResponseMessage? response) = await SendAndGetResponse<TResponse>(@this);
            if (result.Success)
                result.Result = await response.Content.ReadFromJsonAsync<TResponse>();
            else if (result.Error == null && response != null)
                result.Error = await response.Content.ReadAsStringAsync();
            return result;
        }

        public async static Task<HttpResult<string>> SendAsync(this HttpRequest @this)
        {
            (HttpResult<string> result, HttpResponseMessage? response) = await SendAndGetResponse<string>(@this);
            if (result.Success)
                result.Result = await response.Content.ReadAsStringAsync();
            else if (result.Error == null && response != null)
                result.Error = await response.Content.ReadAsStringAsync();
            return result;
        }
    }

    public class HttpRequest : IDisposable
    {
        public HttpClient Http { get; set; }
        public HttpMethod Method { get; set; }
        public string Url { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        public HttpRequest SetParam(string key, string value)
        {
            Parameters[Uri.EscapeDataString(key)] = Uri.EscapeDataString(value);
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
        public TResponse? Result { get; set; }
        public string? Error { get; set; } = null;
    }
}
