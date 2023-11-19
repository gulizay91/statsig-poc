namespace Statsig.Api.Statsig.Exchanges;

public record ExperimentRequest : StatsigRequestBase
{
  public ExperimentRequest(string userId, string experimentName)
  {
    User = NewStatsigUser(userId);
    ParameterName = experimentName;
  }
}