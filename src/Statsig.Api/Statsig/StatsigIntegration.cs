using System.Text.Json;
using Statsig.Api.Statsig.Exchanges;
using Statsig.Server;

namespace Statsig.Api.Statsig;

public class StatsigIntegration : IStatsigIntegration
{
    private readonly ILogger<StatsigIntegration> _logger;

    public StatsigIntegration(ILogger<StatsigIntegration> logger)
    {
        _logger = logger;
    }

    public ServiceResult<bool> CheckGate(FeatureGateRequest request)
    {
        return Execute(() => StatsigServer.CheckGateSync(request.User, request.ParameterName), request);
    }

    public ServiceResult<DynamicConfig> GetExperiment(ExperimentRequest request)
    {
        return Execute(() => StatsigServer.GetExperimentSync(request.User, request.ParameterName), request);
    }

    public ServiceResult<List<string>> GetExperiments()
    {
        return Execute(StatsigServer.GetExperimentList, nameof(GetExperiments));
    }

    public ServiceResult<Layer> GetLayer(LayerRequest request)
    {
        return Execute(() => StatsigServer.GetLayerSync(request.User, request.ParameterName), request);
    }

    public ServiceResult<Dictionary<string, object>> GetClientInitializeResponse(GetClientInitializeResponseRequest request)
    {
        return Execute(() => StatsigServer.GetClientInitializeResponse(request.User), request);
    }

    public void LogEvent(CustomEvent customEvent)
    {
        Execute(() => StatsigServer.LogEvent(customEvent.User, customEvent.EventName, customEvent.Value, customEvent.MetaData), JsonSerializer.Serialize(customEvent));
    }

    private ServiceResult<T> Execute<T>(Func<T> action, object request)
    {
        var requestLog = JsonSerializer.Serialize(request);
        try
        {
            var response = action.Invoke();
            var responseLog = JsonSerializer.Serialize(response);
            _logger.LogInformation("Success, Request: {requestLog}, Response: {responseLog}", requestLog, responseLog);
            return ServiceResult<T>.SuccessResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {ErrorMessage}, Request: {requestLog}", ex.Message, requestLog);
            return ServiceResult<T>.ErrorResult($"An error occurred: {ex.Message}", default);
        }
    }

    private void Execute(Action action, string logMessage)
    {
        try
        {
            action.Invoke();
            _logger.LogInformation("Success: {LogMessage}", logMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error: {LogMessage}", ex.Message);
        }
    }
}