using Statsig.Api.Statsig.Exchanges;

namespace Statsig.Api.Statsig;

public interface IStatsigIntegration
{
  ServiceResult<bool> CheckGate(FeatureGateRequest request);
  ServiceResult<DynamicConfig> GetExperiment(ExperimentRequest request);
  ServiceResult<List<string>?> GetExperiments();
  ServiceResult<Layer> GetLayer(LayerRequest request);
  ServiceResult<Dictionary<string, object>> GetClientInitializeResponse(GetClientInitializeResponseRequest request);
  void LogEvent(CustomEvent customEvent);
}