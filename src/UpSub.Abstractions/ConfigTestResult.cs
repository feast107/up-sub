using System.Net;

namespace UpSub.Abstractions;

public record ConfigTestResult(HttpResponseMessage? Response, HttpRequestError? Error)
{
    public void Deconstruct(out HttpResponseMessage? response, out HttpRequestError? error, out ErrorKind errorKind)
    {
        response  = Response;
        error     = Error;
        errorKind = ErrorKind;
    }

    public ErrorKind ErrorKind => Error switch
    {
        HttpRequestError.ConnectionError       => ErrorKind.ConnectionError,
        HttpRequestError.NameResolutionError   => ErrorKind.NameResolutionError,
        HttpRequestError.SecureConnectionError => ErrorKind.SSLHandshakeFailure,
        null => Response is null
            ? ErrorKind.Cancelled
            : Response.IsSuccessStatusCode
                ? ErrorKind.NoError
                : Response.StatusCode switch
                {
                    HttpStatusCode.NotFound => ErrorKind.NotFound,
                    _                       => ErrorKind.Unknown
                },
        _ => ErrorKind.Unknown
    };
}

public enum ErrorKind
{
    Unknown = -1,
    NoError,
    Cancelled,
    ConnectionError,
    NameResolutionError,
    NotFound,
    SSLHandshakeFailure
}
