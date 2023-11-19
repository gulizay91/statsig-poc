namespace Statsig.Api.Statsig.Exchanges;

public record FeatureGateRequest : StatsigRequestBase
{
  public FeatureGateRequest(string userId, string featureGateName)
  {
    User = NewStatsigUser(userId);
    ParameterName = featureGateName;
  }
}