
namespace Statsig.Api.Registrations;

public static class SwaggerRegister
{
  public static void RegisterSwagger(this IServiceCollection serviceCollection)
  {
    serviceCollection.AddEndpointsApiExplorer();
    serviceCollection.AddSwaggerGen();
  }
}