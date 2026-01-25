using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Planara.Common.Validators;

public static class ValidatorExtension
{
    /// <summary>
    /// Регистрирует FluentValidation-валидаторы из указанных сборок.
    /// </summary>
    public static IServiceCollection AddValidators(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        if (assemblies is null || assemblies.Length == 0)
            throw new ArgumentException("At least one assembly must be provided.", nameof(assemblies));

        services.AddValidatorsFromAssemblies(assemblies);
        return services;
    }
}