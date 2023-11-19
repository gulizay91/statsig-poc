using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Statsig.Api.Settings.Validations;

public class StatsigSettingsValidation : IValidateOptions<StatsigSettings>
{
  private readonly ILogger<StatsigSettingsValidation> _logger;

  public StatsigSettingsValidation(ILogger<StatsigSettingsValidation> logger)
  {
    _logger = logger;
  }

  public ValidateOptionsResult Validate(string name, StatsigSettings options)
  {
    _logger.LogTrace($"{nameof(StatsigSettings)}:{JsonSerializer.Serialize(options)}");

    var resultStatsigSettings = ValidateOptionsForStatsigSettings(options);
    if (resultStatsigSettings is not null) return resultStatsigSettings;

    return ValidateOptionsResult.Success;
  }
  
  private ValidateOptionsResult? ValidateOptionsForStatsigSettings(StatsigSettings options)
  {
    ArgumentNullException.ThrowIfNull(options);

    // if you want to use HttpApi, you have to set url
    // if (string.IsNullOrWhiteSpace(options.Url))
    // {
    //   _logger.LogError(
    //     $"{options.GetType().Name}:{nameof(options.Url)} is null");
    //   return ValidateOptionsResult.Fail(
    //     $"{options.GetType().Name}:{nameof(options.Url)} is null");
    // }
        
    if (string.IsNullOrWhiteSpace(options.ApiKey))
    {
      _logger.LogError(
        $"{options.GetType().Name}:{nameof(options.ApiKey)} is null");
      return ValidateOptionsResult.Fail(
        $"{options.GetType().Name}:{nameof(options.ApiKey)} is null");
    }

    return null;
  }

  
}