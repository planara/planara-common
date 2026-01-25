using System.Net;

namespace Planara.Common.Exceptions;

public sealed class InvalidCredentialsException()
    : BaseException("Invalid credentials", "AUTH_INVALID_CREDENTIALS", HttpStatusCode.Unauthorized);
    
public sealed class InvalidRefreshTokenException()
    : BaseException("Invalid refresh token", "AUTH_INVALID_REFRESH_TOKEN", HttpStatusCode.Unauthorized);
    
public sealed class RefreshTokenRevokedException()
    : BaseException("Refresh token revoked", "AUTH_REFRESH_TOKEN_REVOKED", HttpStatusCode.Unauthorized);

public sealed class RefreshTokenExpiredException()
    : BaseException("Refresh token expired", "AUTH_REFRESH_TOKEN_EXPIRED", HttpStatusCode.Unauthorized);

public sealed class EmailAlreadyRegisteredException()
    : BaseException("Email already registered", "AUTH_EMAIL_ALREADY_REGISTERED", HttpStatusCode.Conflict);