namespace Planara.Common.Auth.Roles;

public static class RoleTypeExtensions
{
    public static string ToJwtValue(this RoleType role) => role switch
    {
        RoleType.User => "user",
        RoleType.Admin => "admin",
        _ => throw new ArgumentOutOfRangeException(nameof(role))
    };

    public static RoleType FromJwtValue(string value) => value.ToLowerInvariant() switch
    {
        "user" => RoleType.User,
        "admin" => RoleType.Admin,
        _ => throw new InvalidOperationException($"Unknown role: {value}")
    };
}
