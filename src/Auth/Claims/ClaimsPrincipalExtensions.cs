using System.Security.Claims;
using Planara.Common.Auth.Roles;

namespace Planara.Common.Auth.Claims;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Получает идентификатор пользователя из клеймов текущего пользователя.
    /// </summary>
    /// <param name="claims">Объект <see cref="ClaimsPrincipal"/>, содержащий клеймы пользователя.</param>
    /// <returns>Идентификатор пользователя в формате <see cref="Guid"/>.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Генерируется, если клейм отсутствует или имеет некорректный формат, не приводимый к <see cref="Guid"/>.
    /// </exception>
    public static Guid GetUserId(this ClaimsPrincipal claims)
    {
        var raw = claims.FindFirst(ClaimTypes.UserId)?.Value;

        if (!Guid.TryParse(raw, out var id))
            throw new UnauthorizedAccessException("Missing or invalid user id claim.");

        return id;
    }

    /// <summary>
    /// Получает роль из клеймов текущего пользователя.
    /// </summary>
    /// <param name="claims">Объект <see cref="ClaimsPrincipal"/>, содержащий клеймы пользователя.</param>
    /// <returns>Роль пользователя <see cref="RoleType"/>.</returns>
    /// <exception cref="UnauthorizedAccessException">
    /// Генерируется, если клейм <c>ClaimTypes.Role</c> отсутствует или имеет неверный формат.
    /// </exception>
    public static RoleType GetRole(this ClaimsPrincipal claims)
    {
        var raw = claims.FindFirst(ClaimTypes.Role)?.Value;

        if (string.IsNullOrWhiteSpace(raw))
            return RoleType.User;

        if (Enum.TryParse<RoleType>(raw, ignoreCase: true, out var role))
            return role;

        throw new UnauthorizedAccessException("Invalid role claim.");
    }

    public static bool HasRole(this ClaimsPrincipal user, RoleType role)
        => user.IsInRole(role.ToString());
}