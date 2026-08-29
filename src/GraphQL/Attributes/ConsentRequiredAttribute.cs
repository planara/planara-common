using System.Reflection;
using HotChocolate.Types.Descriptors;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Planara.Common.Auth.Claims;
using Planara.Common.Database.Domain;
using Planara.Common.Enums;

namespace Planara.Common.GraphQL.Attributes;

/// <summary>
/// Проверка наличия действующего согласия указанного типа
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
public class ConsentRequiredAttribute(ConsentType consentType) : ObjectFieldDescriptorAttribute
{
    protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
    {
        descriptor.Use(next => async middlewareContext =>
        {
            var httpContext = middlewareContext.Services.GetRequiredService<IHttpContextAccessor>().HttpContext;

            if (httpContext?.User.Identity?.IsAuthenticated != true)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage("Authentication is required.")
                    .SetCode("AUTHENTICATION_REQUIRED")
                    .Build());
            }

            var userId = httpContext.User.GetUserId();

            var dataContext = middlewareContext.Services.GetRequiredService<DbContext>();

            var isGranted = await dataContext
                .Set<UserConsentProjection>()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.Type == consentType &&
                    x.IsGranted,
                    middlewareContext.RequestAborted);

            if (!isGranted)
            {
                throw new GraphQLException(ErrorBuilder.New()
                    .SetMessage($"Consent '{consentType}' is required.")
                    .SetCode("CONSENT_REQUIRED")
                    .SetExtension("consentType", consentType.ToString())
                    .Build());
            }

            await next(middlewareContext);
        });
    }
}