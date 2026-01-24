using System.Net;

namespace Planara.Common.Exceptions;

public sealed class InvalidCredentialsException()
    : BaseException("Invalid credentials", "AUTH_INVALID_CREDENTIALS", HttpStatusCode.Unauthorized);