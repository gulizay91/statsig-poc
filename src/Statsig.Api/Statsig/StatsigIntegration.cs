namespace Statsig.Api.Statsig;

public class StatsigIntegration : StatsigIntegrationBase, IStatsigIntegration
{
  private readonly ILogger<StatsigIntegration> _logger;
  public StatsigIntegration(ILogger<StatsigIntegration> logger) : base(logger)
  {
    _logger = logger;
  }
}