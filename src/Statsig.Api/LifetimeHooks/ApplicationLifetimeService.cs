using Statsig.Server;

namespace Statsig.Api.LifetimeHooks;

public class ApplicationLifetimeService : IHostedService
{
  private readonly IHostApplicationLifetime _applicationLifetime;
  private readonly IConfiguration _configuration;
  private readonly ILogger<ApplicationLifetimeService> _logger;

  private bool _statsigInitialized;
  public bool StatsigInitialized => _statsigInitialized;

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
      await InitializeStatsigAsync();
    });

    // register a callback that sleeps for 30 seconds
    _applicationLifetime.ApplicationStopping.Register(async () =>
    {
      _logger.LogInformation("SIGTERM received, waiting for 30 seconds");
      await StatsigServer.Shutdown();
      Thread.Sleep(30_000);
      _logger.LogInformation("Termination delay complete, continuing stopping process");
    });

    return Task.CompletedTask;
  }

  // Required to satisfy interface
  public Task StopAsync(CancellationToken cancellationToken)
  {
    _statsigInitialized = false; // Set to false during shutdown
    return Task.CompletedTask;
  }

  private async Task InitializeStatsigAsync()
  {
    try
    {
      var apiUrl = _configuration.GetSection("StatsigSettings:Url").Value;
      var apiKey = _configuration.GetSection("StatsigSettings:ApiKey").Value;

      if (string.IsNullOrEmpty(apiKey))
      {
        _logger.LogError("StatsigSettings:ApiKey is missing or empty");
        return;
      }

      _logger.LogInformation("StatsigSettings:Url: {Url}", apiUrl);
      _logger.LogInformation("StatsigSettings:ApiKey: {ApiKey}", apiKey);

      var options = new StatsigServerOptions(environment: new StatsigEnvironment(EnvironmentTier.Production));
      await StatsigServer.Initialize(apiKey, options);

      _statsigInitialized = true;
      _logger.LogInformation("Registered Statsig...");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error initializing Statsig");
      _statsigInitialized = false; // Set to false on initialization failure
    }
  }
}