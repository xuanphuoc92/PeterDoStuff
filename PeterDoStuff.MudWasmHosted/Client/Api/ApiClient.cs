using PeterDoStuff.Extensions;
using PeterDoStuff.MudWasmHosted.Client.Extensions;
using System.Reflection.Metadata.Ecma335;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public static class ApiClientExtension
    {
        public static TApiClient As<TApiClient>(this HttpClient http) 
            where TApiClient : ApiClient, new()
        {
            return ApiClient.As<TApiClient>(http);
        }
    }
    
    public abstract class ApiClient
    {
        private HttpClient Http { get; set; }

        protected ApiClient() { }

        internal static TApiClient As<TApiClient>(HttpClient http) where TApiClient : ApiClient, new()
        {
            TApiClient client = new TApiClient();
            client.Http = http;
            return client;
        }

        protected virtual string Route { get; } = string.Empty;

        protected async Task<TReturn> SendToApi<TReturn>(string url, object body = null, params (string Key, string Value)[] headers)
        {
            string route = GetRoute(url);

            HttpRequestBuilder request = Http.Request(HttpMethod.Post, route);

            if (body != null)
                request.SetBody(body);

            foreach (var header in headers)
                request.SetHeader(header.Key, header.Value);

            var result = await request.SendAsync<TReturn>();

            if (result.Success)
                return result.Result;
            throw new ApiException($"Api Error: {result.Error}");
        }

        protected async Task<TReturn> SendToApiAsGet<TReturn>(string url, params (string Key, string Value)[] queryStrings)
        {
            string route = GetRoute(url);

            HttpRequestBuilder request = Http.Request(HttpMethod.Get, route);

            foreach (var queryString in queryStrings)
                request.SetParam(queryString.Key, queryString.Value);

            var result = await request.SendAsync<TReturn>();

            if (result.Success)
                return result.Result;
            throw new ApiException($"Api Error: {result.Error}");
        }

        private string GetRoute(string url)
        {
            var route = Route.IsNullOrEmpty()
                            ? string.Empty
                            : Route.EndsWith('/')
                            ? Route
                            : Route + '/';

            route += url;
            return route;
        }
    }

    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }
}
