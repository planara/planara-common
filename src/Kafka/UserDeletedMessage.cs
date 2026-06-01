namespace Planara.Common.Kafka;

/// <summary>
/// Сообщение об удалении пользователя (топик auth)
/// </summary>
public class UserDeletedMessage
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }
}