namespace Planara.Common.Kafka;

/// <summary>
/// Сообщение о создании нового пользователя (топик auth)
/// </summary>
public class UserCreatedMessage
{
    /// <summary>
    /// ID пользователя
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Адрес электронной почты
    /// </summary>
    public required string Email { get; set; }
}