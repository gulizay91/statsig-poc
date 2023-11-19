using Statsig.Api.Statsig.Exchanges;
using Statsig.Server;

namespace Statsig.Api.Statsig;

public abstract class StatsigIntegrationBase
{
  private readonly ILogger<StatsigIntegrationBase> _logger;
  
  protected StatsigIntegrationBase(ILogger<StatsigIntegrationBase> logger)
  {
    _logger = logger;
  }
  
  public ServiceResult<bool> CheckGate(FeatureGateRequest request)
  {
    var response = StatsigServer.CheckGateSync(request.User, request.ParameterName);
    _logger.LogInformation("User({0}) {1} the gate({2})", request.User.UserID, response ? "pass":"fail", request.ParameterName);
    return ServiceResult<bool>.SuccessResult(response);
  }
  
  public ServiceResult<DynamicConfig> GetExperiment(ExperimentRequest request)
  {
    var response = StatsigServer.GetExperimentSync(request.User, request.ParameterName);
    return ServiceResult<DynamicConfig>.SuccessResult(response);
  }
  
  public ServiceResult<List<string>?> GetExperiments()
  {
    var response = StatsigServer.GetExperimentList();
    return ServiceResult<List<string>?>.SuccessResult(response);
  }
  
  public ServiceResult<Layer> GetLayer(LayerRequest request)
  {
    var response = StatsigServer.GetLayerSync(request.User, request.ParameterName);
    return ServiceResult<Layer>.SuccessResult(response);
  }
  
  public ServiceResult<Dictionary<string, object>> GetClientInitializeResponse(GetClientInitializeResponseRequest request)
  {
    var response = StatsigServer.GetClientInitializeResponse(request.User);
    return ServiceResult<Dictionary<string, object>>.SuccessResult(response);
  }
  
  public void LogEvent(CustomEvent customEvent)
  {
    StatsigServer.LogEvent(customEvent.User, customEvent.EventName, customEvent.Value, customEvent.MetaData);
  }
}