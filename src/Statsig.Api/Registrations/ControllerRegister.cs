using System.Text.Json.Serialization;

namespace Statsig.Api.Registrations;

public static class ControllerRegister
{
  public static void RegisterControllers(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddControllers().AddJsonOptions(options =>
    {
      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    serviceCollection.AddRouting(options => options.LowercaseUrls = true);
  }
}