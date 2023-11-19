using System.Net;

namespace Statsig.Api.Statsig.Exchanges;

public record ServiceResult<T>
{
  public ServiceResult(bool success, string responseMessage, T? data,
    HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    Success = success;
    ResponseMessage = responseMessage;
    Data = data;
    StatusCode = statusCode;
  }

  public T? Data { get; protected set; }

  public string ResponseMessage { get; protected set; }

  public HttpStatusCode StatusCode { get; protected set; }

  public bool Success { get; protected set; }

  public static ServiceResult<T> ErrorResult(string errorMessage, T? payload,
    HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
  {
    return new ServiceResult<T>(false, errorMessage, payload, statusCode);
  }

  public static ServiceResult<T> SuccessResult(T? payload, string? message = null,
    HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    return new ServiceResult<T>(true, message ?? "Process completed successfully.", payload, statusCode);
  }
}