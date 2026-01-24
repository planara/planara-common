using System.Net;

namespace Planara.Common.Exceptions;

public sealed class NotFoundException()
    : BaseException("Resource not found", "NOT_FOUND", HttpStatusCode.NotFound);