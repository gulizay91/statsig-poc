using Statsig.Server;

namespace Statsig.Api.LifetimeHooks;

public class ApplicationLifetimeService : IHostedService
{
  private readonly IHostApplicationLifetime _applicationLifetime;
  private readonly IConfiguration _configuration;
  private readonly ILogger<ApplicationLifetimeService> _logger;

  public ApplicationLifetimeService(IHostApplicationLifetime applicationLifetime,
    ILogger<ApplicationLifetimeService> logger, IConfiguration configuration)
  {
    _applicationLifetime = applicationLifetime;
    _logger = logger;
    _configuration = configuration;
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _applicationLifetime.ApplicationStarted.Register(async () =>
    {
      _logger.LogInformation("Termination delay complete, continuing stopping process");
      await InitStatsig();
    });
    // register a callback that sleeps for 30 seconds
    _applicationLifetime.ApplicationStopping.Register(() =>
    {
      _logger.LogInformation("SIGTERM received, waiting for 30 seconds");
      Thread.Sleep(30_000);
      _logger.LogInformation("Termination delay complete, continuing stopping process");
    });
    return Task.CompletedTask;
  }

  // Required to satisfy interface
  public Task StopAsync(CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  private async Task InitStatsig()
  {
    var apiUrl = _configuration.GetSection("StatsigSettings:Url").Value;
    Console.Out.WriteLine($"StatsigSettings:Url: {apiUrl}");
    var apiKey = _configuration.GetSection("StatsigSettings:ApiKey").Value;
    Console.Out.WriteLine($"StatsigSettings:ApiKey: {apiKey}");
    var options = new StatsigServerOptions(environment: new StatsigEnvironment(EnvironmentTier.Production));
    await StatsigServer.Initialize(apiKey, options);
    Console.Out.WriteLine("Registered Statsig...");
  }
}