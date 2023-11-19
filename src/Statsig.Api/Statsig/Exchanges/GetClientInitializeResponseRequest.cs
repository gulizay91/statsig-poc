namespace Statsig.Api.Statsig.Exchanges;

public record GetClientInitializeResponseRequest : StatsigRequestBase
{
  public GetClientInitializeResponseRequest(string userId)
  {
    User = NewStatsigUser(userId);
  }
}