using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ClyvoCareOSChallenge.Authentication;

public sealed class ApiKeyAuthenticationOptions
    : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = string.Empty;
}

public sealed class ApiKeyAuthenticationHandler
    : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    public const string SchemeName = "ApiKey";
    public const string HeaderName = "X-API-Key";

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult>
        HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(
                HeaderName,
                out StringValues apiKeyRecebida))
        {
            return Task.FromResult(
                AuthenticateResult.NoResult()
            );
        }

        if (string.IsNullOrWhiteSpace(Options.ApiKey))
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "API Key não configurada."
                )
            );
        }

        var chavesSaoIguais = string.Equals(
            apiKeyRecebida.ToString(),
            Options.ApiKey,
            StringComparison.Ordinal
        );

        if (!chavesSaoIguais)
        {
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "API Key inválida."
                )
            );
        }

        Claim[] claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                "clyvocare-client"
            ),
            new Claim(
                ClaimTypes.Name,
                "Cliente ClyvoCare"
            )
        };

        var identity = new ClaimsIdentity(
            claims,
            SchemeName
        );

        var principal =
            new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(
            principal,
            SchemeName
        );

        return Task.FromResult(
            AuthenticateResult.Success(ticket)
        );
    }
}