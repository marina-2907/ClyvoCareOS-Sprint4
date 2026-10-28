using System.Net;

namespace ClyvoCareOSChallenge.IntegrationTests;

[Collection("ApiCollection")]
public class ApiIntegrationTests
{
    private readonly HttpClient _client;

    public ApiIntegrationTests(ApiFixture fixture)
    {
        _client = fixture.CriarCliente();
    }

    [Fact]
    public async Task HealthLive_QuandoApiEstaDisponivel_RetornaStatus200()
    {
        // Arrange
        const string endpoint = "/health/live";

        // Act
        var response = await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        var conteudo =
            await response.Content.ReadAsStringAsync();

        Assert.Equal("Healthy", conteudo);
    }

    [Fact]
    public async Task CorrelationId_QuandoEnviado_RetornaMesmoValor()
    {
        // Arrange
        const string correlationId =
            "teste-integracao-clyvocare";

        using var requisicao = new HttpRequestMessage(
            HttpMethod.Get,
            "/health/live"
        );

        requisicao.Headers.Add(
            "X-Correlation-ID",
            correlationId
        );

        // Act
        var response =
            await _client.SendAsync(requisicao);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );

        Assert.True(
            response.Headers.TryGetValues(
                "X-Correlation-ID",
                out var valores
            )
        );

        Assert.Equal(
            correlationId,
            valores.Single()
        );
    }

    [Fact]
    public async Task EndpointInexistente_QuandoAcessado_Retorna404NotFound()
    {
        // Arrange
        const string endpoint =
            "/api/endpoint-inexistente";

        // Act
        var response =
            await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode
        );
    }

    [Fact]
    public async Task EndpointProtegido_SemApiKey_Retorna401Unauthorized()
    {
        // Arrange
        const string endpoint =
            "/api/seguranca/protegido";

        // Act
        var response =
            await _client.GetAsync(endpoint);

        // Assert
        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode
        );
    }

    [Fact]
    public async Task EndpointProtegido_ComApiKeyValida_Retorna200OK()
    {
        // Arrange
        using var requisicao = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/seguranca/protegido"
        );

        requisicao.Headers.Add(
            "X-API-Key",
            ApiFixture.ApiKeyDeTeste
        );

        // Act
        var response =
            await _client.SendAsync(requisicao);

        // Assert
        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode
        );
    }
}