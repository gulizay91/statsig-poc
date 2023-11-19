namespace Statsig.Api.Statsig.Exchanges;

public record CustomEvent : StatsigRequestBase
{
  public CustomEvent(StatsigUser user, string eventName, string? value = null, Dictionary<string, string>? metaData = null)
  {
    EventName = eventName;
    User = user;
    Value = value;
    MetaData = metaData;
  }
  
  public string EventName { get; init; }
  public string? Value { get; set; }
  public Dictionary<string, string>? MetaData { get; set; }
}