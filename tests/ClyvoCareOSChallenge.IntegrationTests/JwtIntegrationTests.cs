using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ClyvoCareOSChallenge.IntegrationTests;

public class JwtIntegrationTests
{
    [Fact]
    public async Task Validar_SemJwt_DeveRetornar401()
    {
        // Arrange
        using var factory = new ApiFixture();
        using var client = factory.CriarCliente();
        // Act
        var response = await client.GetAsync("/api/auth/validar");
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
