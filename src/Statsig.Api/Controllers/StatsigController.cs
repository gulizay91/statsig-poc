using Microsoft.AspNetCore.Mvc;
using Statsig.Api.Extensions;
using Statsig.Api.Statsig;
using Statsig.Api.Statsig.Exchanges;

namespace Statsig.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatsigController : ControllerBase
{
  private readonly IStatsigIntegration _statsigIntegration;
  private readonly ILogger<StatsigController> _logger;

  public StatsigController(ILogger<StatsigController> logger, IStatsigIntegration statsigIntegration)
  {
    _logger = logger;
    _statsigIntegration = statsigIntegration;
  }

  [HttpGet]
  [Route("check-gate")]
  public IActionResult GetCheckGate([FromHeader(Name = "user-id")] string userId, [FromQuery] string featureGateName)
  {
    var request = new FeatureGateRequest(userId, featureGateName);
    _logger.LogInformation("Check the gate({0}) for User({1}) ", featureGateName, userId);
    var result = _statsigIntegration.CheckGate(request);
    return Ok(result);
  }
  
  [HttpGet]
  [Route("experiments")]
  public IActionResult GetExperiment([FromHeader(Name = "user-id")] string userId, [FromQuery] string? experimentName)
  {
    if (!string.IsNullOrWhiteSpace(experimentName))
    {
      var request = new ExperimentRequest(userId, experimentName);
      var resultExperiment = _statsigIntegration.GetExperiment(request);
      return Ok(resultExperiment);
    }

    var resultExperiments = _statsigIntegration.GetExperiments();
    return Ok(resultExperiments);
  }
  
  [HttpGet]
  [Route("layer")]
  public IActionResult GetLayer([FromHeader(Name = "user-id")] string userId, [FromQuery] string layerName)
  {
    var request = new LayerRequest(userId, layerName);
    var result = _statsigIntegration.GetLayer(request);
    return Ok(result);
  }
  
  [HttpGet]
  [Route("get-client-initialize-response")]
  public IActionResult GetClientInitializeResponse([FromHeader(Name = "user-id")] string userId)
  {
    var request = new GetClientInitializeResponseRequest(userId);
    var result = _statsigIntegration.GetClientInitializeResponse(request);
    return Ok(result);
  }
  
  [HttpGet]
  [Route("check-creditcard")]
  public IActionResult CheckCreditCard([FromHeader(Name = "user-id")] string userId, [FromQuery] string creditCard)
  {
    var maskedCard = CreditCardExtensions.Mask(creditCard);
    const string featureGateName = "mastercard";
    var request = new FeatureGateRequest(userId, featureGateName);
    request.User.AddPrivateAttribute("creditCard", creditCard);
    request.User.AddCustomProperty("maskedCreditCard", maskedCard);
    _logger.LogInformation("Check the gate({0}) for User({1}) ", featureGateName, userId);
    var result = _statsigIntegration.CheckGate(request);
    if (result.Data)
    {
      var customEvent = new CustomEvent(request.User, "MasterCardCustomEvent", $"sku-{Guid.NewGuid().ToString()}");
      _statsigIntegration.LogEvent(customEvent);
      _logger.LogInformation("CustomEvent({0}) for User({1}) ", customEvent.EventName, userId);
    }
    
    return Ok(result);
  }
}