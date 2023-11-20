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
    try
    {
      var response = StatsigServer.CheckGateSync(request.User, request.ParameterName);
      _logger.LogInformation("User({0}) {1} the gate({2})", request.User.UserID, response ? "pass" : "fail",
        request.ParameterName);
      return ServiceResult<bool>.SuccessResult(response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in CheckGate for user {UserId}, gate {GateName}", request.User.UserID,
        request.ParameterName);
      return ServiceResult<bool>.ErrorResult("An error occurred while checking the gate.", false);
    }
  }

  public ServiceResult<DynamicConfig> GetExperiment(ExperimentRequest request)
  {
    try
    {
      var response = StatsigServer.GetExperimentSync(request.User, request.ParameterName);
      return ServiceResult<DynamicConfig>.SuccessResult(response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in GetExperiment for user {UserId}, parameter {ParameterName}", request.User.UserID,
        request.ParameterName);
      return ServiceResult<DynamicConfig>.ErrorResult("An error occurred while getting the experiment.", null);
    }
  }

  public ServiceResult<List<string>?> GetExperiments()
  {
    try
    {
      var response = StatsigServer.GetExperimentList();
      return ServiceResult<List<string>?>.SuccessResult(response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in GetExperiments");
      return ServiceResult<List<string>?>.ErrorResult("An error occurred while getting the experiments.", null);
    }
  }

  public ServiceResult<Layer> GetLayer(LayerRequest request)
  {
    try
    {
      var response = StatsigServer.GetLayerSync(request.User, request.ParameterName);
      return ServiceResult<Layer>.SuccessResult(response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in GetLayer for user {UserId}, parameter {ParameterName}", request.User.UserID,
        request.ParameterName);
      return ServiceResult<Layer>.ErrorResult("An error occurred while getting the layer.", null);
    }
  }

  public ServiceResult<Dictionary<string, object>> GetClientInitializeResponse(
    GetClientInitializeResponseRequest request)
  {
    try
    {
      var response = StatsigServer.GetClientInitializeResponse(request.User);
      return ServiceResult<Dictionary<string, object>>.SuccessResult(response);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in GetClientInitializeResponse for user {UserId}", request.User.UserID);
      return ServiceResult<Dictionary<string, object>>.ErrorResult(
        "An error occurred while getting the client initialize response.", null);
    }
  }

  public void LogEvent(CustomEvent customEvent)
  {
    try
    {
      StatsigServer.LogEvent(customEvent.User, customEvent.EventName, customEvent.Value, customEvent.MetaData);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error in LogEvent for user {UserId}, event {EventName}", customEvent.User.UserID,
        customEvent.EventName);
    }
  }
}