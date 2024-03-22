using System.Net;

namespace PeterDoStuff.MudWasmHosted.Server.Auth
{
    public class ReferrerValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _allowedReferrer;

        public ReferrerValidationMiddleware(RequestDelegate next, string allowedReferrer)
        {
            _next = next;
            _allowedReferrer = allowedReferrer;
        }

        public async Task Invoke(HttpContext context)
        {
            var isApiRequest = false;
            isApiRequest |= context.Request.Path.Value?.StartsWith("/api/") ?? false;
            isApiRequest |= context.Request.Path.Value?.StartsWith("/_smartcomponents/") ?? false;

            var referrer = context.Request.Headers["Referer"].ToString();
            if (isApiRequest && referrer.StartsWith(_allowedReferrer) == false)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _next(context);
        }
    }

}
