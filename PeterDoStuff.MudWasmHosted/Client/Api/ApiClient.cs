using PeterDoStuff.Extensions;
using PeterDoStuff.MudWasmHosted.Client.Extensions;
using System.Reflection.Metadata.Ecma335;

namespace PeterDoStuff.MudWasmHosted.Client.Api
{
    public static class ApiClientExtension
    {
        public static TApiClient As<TApiClient>(this HttpClient client) 
            where TApiClient : ApiClient, new()
        {
            return ApiClient.As<TApiClient>(client);
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

        protected async Task<TReturn> SendToApi<TReturn>(string url, object body)
        {
            var route = Route.IsNullOrEmpty()
                ? string.Empty
                : Route.EndsWith('/')
                ? Route
                : Route + '/';

            var result = await Http.Request(HttpMethod.Post, route + url)
                .SetBody(body)
                .SendAsync<TReturn>();
            
            if (result.Success)
                return result.Result;
            throw new ApiException($"Api Error: {result.Error}");
        }
    }

    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }
}
