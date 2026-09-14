using System.Net;

namespace Ciam.Application.Common.Exceptions;

public sealed class IdentityProviderException(
    string operation,
    HttpStatusCode statusCode,
    string? providerReason = null)
    : Exception($"The identity provider rejected the {operation} request.")
{
    public string Operation { get; } = operation;

    public HttpStatusCode StatusCode { get; } = statusCode;

    public string? ProviderReason { get; } = providerReason;
}
