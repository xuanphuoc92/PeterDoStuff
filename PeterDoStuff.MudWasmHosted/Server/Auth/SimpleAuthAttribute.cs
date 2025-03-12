using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PeterDoStuff.MudWasmHosted.Server.Auth
{
    public class SimpleAuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _environmentVariableKey, _defaultValue;
        private const string AUTH_HEADER = "SimpleAuth";
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="environmentVariableKey"></param>
        /// <param name="defaultValue"></param>
        public SimpleAuthAttribute(string environmentVariableKey, string? defaultValue = null)
        {
            _environmentVariableKey = environmentVariableKey;
            _defaultValue = defaultValue;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var accessKey = Environment.GetEnvironmentVariable(_environmentVariableKey)
                ?? _defaultValue;

            if (accessKey == null)
            {
                context.Result = new UnauthorizedObjectResult($"Set Environment Variable '{_environmentVariableKey}' before you can use this attribute");
                return;
            }

            if (context.HttpContext.Request.Headers.TryGetValue(AUTH_HEADER, out var authHeader) == false)
            {
                context.Result = new UnauthorizedObjectResult($"Set the header '{AUTH_HEADER}' to use this attribute");
                return;
            }

            if (authHeader != accessKey)
            {
                context.Result = new UnauthorizedObjectResult("Invalid header");
                return;
            }
        }
    }
}
