using Microsoft.Extensions.Options;
using Statsig.Api.Settings;
using Statsig.Api.Settings.Validations;

namespace Statsig.Api.Registrations;

public static class SettingsRegister
{
  public static void RegisterSettings(this IServiceCollection serviceCollection,IConfiguration configuration)
  {
    serviceCollection.AddOptions<StatsigSettings>().ValidateOnStart();

    serviceCollection.Configure<StatsigSettings>(configuration.GetSection(nameof(StatsigSettings)));
    var statsigSettings=configuration.Get<StatsigSettings>();
    configuration.GetSection(nameof(StatsigSettings)).Bind(statsigSettings);
    serviceCollection.AddSingleton(statsigSettings);

    serviceCollection.AddSingleton<IValidateOptions<StatsigSettings>, StatsigSettingsValidation>();
  }
}