using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Statsig.Api.Middlewares.MiddlewareModels;
using Statsig.Lib;

namespace Statsig.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
  private readonly ILogger<ExceptionHandlerMiddleware> _logger;
  private readonly RequestDelegate _next;

  public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
  {
    _logger = logger;
    _next = next;
  }

  public async Task InvokeAsync(HttpContext httpContext)
  {
    try
    {
      await _next(httpContext);
    }
    catch (Exception ex)
    {
      var message = string.IsNullOrWhiteSpace(ex.Message)
        ? "Internal Server Error from the custom middleware."
        : ex.Message;
      _logger.LogError(message, ex, null, null, new HttpMethod(httpContext.Request.Method),
        (HttpStatusCode)httpContext.Response.StatusCode, null, httpContext.Request.Host.Value,
        httpContext.Request.Path);
      await HandleExceptionAsync(httpContext, ex);
    }
  }

  private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
  {
    var message = string.IsNullOrWhiteSpace(exception.Message)
      ? "Internal Server Error from the custom middleware."
      : exception.Message;
    httpContext.Response.ContentType = MediaTypeNames.Application.Json;
    httpContext.Response.StatusCode = exception switch
    {
      _ => (int)HttpStatusCode.InternalServerError
    };
    message = exception switch
    {
      StatsigUninitializedException => "You have to initialize statsig!",
      _ => message
    };

    var result = JsonSerializer.Serialize(new ErrorResponse { Message = message });
    //var result = JsonSerializer.Serialize(new Microsoft.AspNetCore.Mvc.ProblemDetails { Detail = message });
    await httpContext.Response.WriteAsync(result);
  }
}