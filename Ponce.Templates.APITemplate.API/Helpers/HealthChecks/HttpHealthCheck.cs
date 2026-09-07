using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ponce.Templates.APITemplate.API.Helpers.HealthChecks;

/// <summary>Generic health check for an external HTTP dependency, with an optional login step.</summary>
public class HttpHealthCheck(string url, string urlLogin = "", object? loginRequest = null) : IHealthCheck
{
    private readonly string _url = url;
    private readonly string _urlLogin = urlLogin;
    private readonly object? _loginRequest = loginRequest;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(_url, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Unhealthy("Connection to the service is unavailable.");
            }
            if (!string.IsNullOrEmpty(_urlLogin) && _loginRequest != null)
            {
                var content = new StringContent(JsonSerializer.Serialize(_loginRequest), Encoding.UTF8, "application/json");
                var loginResponse = await httpClient.PostAsync(_urlLogin, content, cancellationToken);
                if (!loginResponse.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Unhealthy("Access to the service was denied.");
                }
            }
            return HealthCheckResult.Healthy("Connection to the service is available.");
        }
        catch
        {
            return HealthCheckResult.Unhealthy("Connection to the service is unavailable.");
        }
    }
}
