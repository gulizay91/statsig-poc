using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Statsig.Api.Registrations;

public static class HealthCheckRegister
{
  public static void RegisterHealthCheck(this IApplicationBuilder applicationBuilder)
  {
    //for liveness probe
    applicationBuilder.UseEndpoints(endpoints =>
    {
      endpoints.MapHealthChecks("/health", new HealthCheckOptions
      {
        Predicate = _ => false
      });
    });

    //for readiness probe. 
    applicationBuilder.UseEndpoints(endpoints =>
    {
      endpoints.MapHealthChecks("/ready", new HealthCheckOptions
      {
        Predicate = check => check.Tags.Contains("ready")
      });
    });
  }
}