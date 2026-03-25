using System.Net;

namespace Backend.Infrastructure.Api;

public sealed class ApiException : Exception
{
    public ApiException(HttpStatusCode statusCode, string message, string? code = null, object? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Details = details;
    }

    public HttpStatusCode StatusCode { get; }
    public string? Code { get; }
    public object? Details { get; }
}

