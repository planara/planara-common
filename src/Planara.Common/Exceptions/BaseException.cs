using System.Net;

namespace Planara.Common.Exceptions;

public abstract class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string Code { get; }

    protected BaseException(string message,
        string code,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        Code = code;
    }
}
