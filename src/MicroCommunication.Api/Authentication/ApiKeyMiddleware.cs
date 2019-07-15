using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MicroCommunication.Api.Authentication
{
    /// <summary>
    /// An ASP.NET Core Middleware, that makes sure, an API Key is set in every call that gets
    /// sent to an endpoint.
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly string apiKeyHeaderName = "api-key";
        private readonly RequestDelegate next;
        private string apiKey { get; set; }

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            this.next = next;
            this.apiKey = configuration["ApiKey"];
        }

        public async Task Invoke(HttpContext context)
        {
            // Check, if API Header is present, when request contains /api path
            if (context.Request.Path.ToString().Contains("/api"))
            {
                if (!context.Request.Headers.Keys.Contains(apiKeyHeaderName))
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await context.Response.WriteAsync($"API Key is missing. Please add an '{apiKeyHeaderName}' header");
                    return;
                }
                else
                {
                    if (!apiKey.Equals(context.Request.Headers[apiKeyHeaderName]))
                    {
                        context.Response.StatusCode = 401; // Unauthorized
                        await context.Response.WriteAsync("Invalid API Key.");
                        return;
                    }
                }
            }

            await next.Invoke(context);
        }
    }

    public static class ApiKeyMiddlewareExtension
    {
        public static IApplicationBuilder UseApiKey(this IApplicationBuilder app)
        {
            app.UseMiddleware<ApiKeyMiddleware>();
            return app;
        }
    }
}
