using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MicroCommunication.Random.Authentication
{
    /// <summary>
    /// An ASP.NET Core Middleware, that makes sure, an API Key is set in every call that gets
    /// sent to an endpoint.
    /// </summary>
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ApiKeyOptions configuration;

        public ApiKeyMiddleware(RequestDelegate next, ApiKeyOptions configuration)
        {
            this.next = next;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check, if current path does not require an API Key
            if (!context.Request.Path.ToString().Contains("/api/"))
            {
                await next.Invoke(context);
                return;
            }

            // Check, if API Header is present, when request contains /api path
            if (!context.Request.Headers.Keys.Contains(configuration.ApiKeyHeaderName))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync(
                    $"API Key is missing. Please add an '{configuration.ApiKeyHeaderName}' header");
                return;
            }

            // Check, if API Key is correct
            if (!configuration.ApiKey.Equals(context.Request.Headers[configuration.ApiKeyHeaderName]))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            await next.Invoke(context);
        }
    }

    public static class ApiKeyMiddlewareExtension
    {
        public static IApplicationBuilder UseApiKey(
            this IApplicationBuilder app,
            Action<ApiKeyOptions> setupAction = null)
        {
            var options = new ApiKeyOptions();
            if (setupAction != null)
                setupAction.Invoke(options);

            app.UseMiddleware<ApiKeyMiddleware>(options);
            return app;
        }
    }

    public class ApiKeyOptions
    {
        public string ApiKeyHeaderName { get; set; } = "api-key";
        public string ApiKey { get; set; }
    }
}
