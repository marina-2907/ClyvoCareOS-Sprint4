using ClyvoCareOSChallenge.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ClyvoCareOSChallenge.IntegrationTests;

public sealed class ApiFixture : WebApplicationFactory<Program>
{
    public const string ApiKeyDeTeste =
        "chave-teste-clyvocare";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.PostConfigure<
                ApiKeyAuthenticationOptions
            >(
                ApiKeyAuthenticationHandler.SchemeName,
                options =>
                {
                    options.ApiKey = ApiKeyDeTeste;
                }
            );
        });
    }

    public HttpClient CriarCliente()
    {
        return CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress =
                    new Uri("https://localhost")
            }
        );
    }
}

[CollectionDefinition("ApiCollection")]
public class ApiCollection :
    ICollectionFixture<ApiFixture>
{
}