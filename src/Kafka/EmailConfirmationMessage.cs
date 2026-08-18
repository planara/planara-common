namespace Planara.Common.Kafka;

/// <summary>
/// Сообщение с кодом подтверждения почты
/// </summary>
public class EmailConfirmationMessage
{
    /// <summary>
    /// Почта пользователя
    /// </summary>
    public required string Email { get; set; }
    
    /// <summary>
    /// Код подтверждения почты
    /// </summary>
    public required string Code { get; init; }
}