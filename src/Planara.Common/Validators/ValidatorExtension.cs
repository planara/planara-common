using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Planara.Common.Validators;

public static class ValidatorExtension
{
    public static void AddValidators(this IServiceCollection services) =>
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
}