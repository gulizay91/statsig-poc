using Statsig.Api.Statsig;

namespace Statsig.Api.Registrations;

public static class ServicesRegister
{
  public static void RegisterServices(this IServiceCollection serviceCollection, IConfiguration configuration)
  {
    serviceCollection.AddSingleton<IStatsigIntegration, StatsigIntegration>();
  }
}