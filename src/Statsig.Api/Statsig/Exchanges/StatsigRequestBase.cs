using System.ComponentModel.DataAnnotations;

namespace Statsig.Api.Statsig.Exchanges;

public record StatsigRequestBase
{
  [Required]
  public StatsigUser User { get; set; }
  public string ParameterName { get; set; }

  protected StatsigUser NewStatsigUser(string userId)
  {
    return new StatsigUser { UserID = userId };
  }
}