using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoCareOSChallenge.HealthChecks;

public class ExternalServiceHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalServiceHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient("ViaCep");

            HttpResponseMessage response = await client.GetAsync(
                "/ws/01001000/json/",
                cancellationToken
            );

            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy(
                    "Serviço externo ViaCEP disponível."
                );
            }

            return HealthCheckResult.Unhealthy(
                $"ViaCEP respondeu com o status {(int)response.StatusCode}."
            );
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                "Não foi possível acessar o ViaCEP.",
                exception
            );
        }
    }
}