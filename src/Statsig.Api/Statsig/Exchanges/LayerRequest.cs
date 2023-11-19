namespace Statsig.Api.Statsig.Exchanges;

public record LayerRequest : StatsigRequestBase
{
  public LayerRequest(string userId, string experimentName)
  {
    User = NewStatsigUser(userId);
    ParameterName = experimentName;
  }
}