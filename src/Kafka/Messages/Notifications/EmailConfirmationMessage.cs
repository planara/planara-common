namespace Planara.Common.Kafka.Messages.Notifications;

/// <summary>
/// Сообщение с кодом подтверждения почты
/// </summary>
public class EmailConfirmationMessage
{
    /// <summary>
    /// ID сообщения
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Почта пользователя
    /// </summary>
    public required string Email { get; set; }
    
    /// <summary>
    /// Код подтверждения почты
    /// </summary>
    public required string Code { get; init; }
}