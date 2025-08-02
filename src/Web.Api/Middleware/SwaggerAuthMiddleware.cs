
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Web.Api.Middleware
{
    [ExcludeFromCodeCoverage]
    public class SwaggerAuthMiddleware
    {
        private readonly RequestDelegate _next;
        public SwaggerAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    var header = AuthenticationHeaderValue.Parse(authHeader);
                    var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter)).Split(':');
                    var username = credentials.FirstOrDefault();
                    var password = credentials.LastOrDefault();
                    var swaggerAuth = configuration.GetSection("SwaggerBasicAuth");

                    if (swaggerAuth["UserName"] == username && swaggerAuth["Password"] == password)
                    {
                        await _next(context);
                        return;
                    }
                }
                context.Response.Headers["WWW-Authenticate"] = "Basic";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                await _next(context);
            }
        }
    }
}
