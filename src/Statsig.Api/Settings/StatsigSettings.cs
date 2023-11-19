namespace Statsig.Api.Settings;

public record StatsigSettings
{
  public string Url { get; set; }
  public string ApiKey { get; set; }
}