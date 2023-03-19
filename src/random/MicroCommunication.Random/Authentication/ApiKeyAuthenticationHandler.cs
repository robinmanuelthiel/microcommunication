using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicroCommunication.Random.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        readonly string apiKeyHeaderName;
        readonly string apiKey;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {
            apiKeyHeaderName = options.CurrentValue.ApiKeyHeaderName;
            apiKey = options.CurrentValue.ApiKey;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity();
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            if (!Request.Headers.TryGetValue(apiKeyHeaderName, out var apiKeyHeaderValues))
            {
                return Task.FromResult(AuthenticateResult.Fail($"API Key is missing. Please add an '{apiKeyHeaderName}' header"));
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();
            if (!apiKeyHeaderValues.Any() || string.IsNullOrEmpty(providedApiKey))
            {
                return Task.FromResult(AuthenticateResult.Fail($"API Key is missing. Please add an '{apiKeyHeaderName}' header"));
            }

            if (providedApiKey.Equals(apiKey))
            {
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            else
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid API Key"));
            }
        }
    }
}
