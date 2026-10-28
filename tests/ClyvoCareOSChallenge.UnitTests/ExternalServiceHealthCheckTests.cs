using System.Net;
using ClyvoCareOSChallenge.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using Moq.Protected;

namespace ClyvoCareOSChallenge.UnitTests;

public class ExternalServiceHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_QuandoViaCepRespondeComSucesso_RetornaHealthy()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage(HttpStatusCode.OK)
            );

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://viacep.com.br")
        };

        var factoryMock = new Mock<IHttpClientFactory>();

        factoryMock
            .Setup(factory => factory.CreateClient("ViaCep"))
            .Returns(httpClient);

        var healthCheck =
            new ExternalServiceHealthCheck(factoryMock.Object);

        // Act
        var resultado = await healthCheck.CheckHealthAsync(
            new HealthCheckContext()
        );

        // Assert
        Assert.Equal(HealthStatus.Healthy, resultado.Status);

        factoryMock.Verify(
            factory => factory.CreateClient("ViaCep"),
            Times.Once
        );
    }

    [Fact]
    public async Task CheckHealthAsync_QuandoViaCepEstaIndisponivel_RetornaUnhealthy()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage(
                    HttpStatusCode.ServiceUnavailable
                )
            );

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://viacep.com.br")
        };

        var factoryMock = new Mock<IHttpClientFactory>();

        factoryMock
            .Setup(factory => factory.CreateClient("ViaCep"))
            .Returns(httpClient);

        var healthCheck =
            new ExternalServiceHealthCheck(factoryMock.Object);

        // Act
        var resultado = await healthCheck.CheckHealthAsync(
            new HealthCheckContext()
        );

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, resultado.Status);

        factoryMock.Verify(
            factory => factory.CreateClient("ViaCep"),
            Times.Once
        );
    }
}