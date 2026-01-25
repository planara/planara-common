using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Planara.Common.Exceptions;

namespace Planara.Common.GraphQL.Filters;

public sealed class ErrorFilter(ILogger<ErrorFilter> logger) : IErrorFilter
{
    public IError OnError(IError error)
    {
        var ex = Unwrap(error.Exception);
        
        switch (ex)
        {
            case ValidationException vex:
            {
                logger.LogWarning(ex, "GraphQL validation error: {Message}", error.Message);
            
                var validationErrors = vex.Errors
                    .Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage,
                        errorCode = e.ErrorCode
                    })
                    .ToList();

                return ErrorBuilder
                    .FromError(error)
                    .SetCode("VALIDATION_FAILED")
                    .SetMessage("Validation failed")
                    .SetExtension("validationErrors", validationErrors)
                    .Build();
            }
            
            case SecurityTokenExpiredException:
                logger.LogWarning(ex, "Token expired: {Message}", error.Message);
                return ErrorBuilder.FromError(error)
                    .SetCode("TOKEN_EXPIRED")
                    .SetMessage("Token expired")
                    .Build();
            
            case SecurityTokenInvalidSignatureException:
                logger.LogWarning(ex, "Invalid token signature: {Message}", error.Message);
                return ErrorBuilder.FromError(error)
                    .SetCode("TOKEN_INVALID_SIGNATURE")
                    .SetMessage("Invalid token signature")
                    .Build();

            case SecurityTokenException:
                logger.LogWarning(ex, "Invalid token: {Message}", error.Message);
                return ErrorBuilder.FromError(error)
                    .SetCode("TOKEN_INVALID")
                    .SetMessage("Invalid token")
                    .Build();
            
            case UnauthorizedAccessException:
                logger.LogWarning(ex, "Unauthorized: {Message}", error.Message);
                return ErrorBuilder
                    .FromError(error)
                    .SetCode("UNAUTHORIZED")
                    .SetMessage("Unauthorized")
                    .Build();
            
            case BaseException bex when !string.IsNullOrWhiteSpace(bex.Code):
                logger.LogError(ex, "GraphQL Error: {Message}", error.Message);
                return ErrorBuilder
                    .FromError(error)
                    .SetCode(bex.Code)
                    .SetMessage(bex.Message)
                    .Build();
            default:
                return error;
        }
    }

    private static Exception? Unwrap(Exception? ex)
    {
        if (ex is null) return null;

        if (ex is AggregateException aex && aex.InnerExceptions.Count == 1)
            return Unwrap(aex.InnerExceptions[0]);

        if (ex.InnerException is not null &&
            ex is System.Reflection.TargetInvocationException)
            return Unwrap(ex.InnerException);

        return ex;
    }
}
