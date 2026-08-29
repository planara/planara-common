using Planara.Common.Enums;

namespace Planara.Common.Kafka.Messages.Privacy;

/// <summary>
/// Запрос на фиксацию согласия пользователя
/// </summary>
public class ConsentGrantRequestedMessage
{
    /// <summary>
    /// Уникальный идентификатор запроса на выдачу согласия
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Идентификатор регистрационной сессии, если согласие выдаётся до создания постоянного пользователя
    /// </summary>
    public Guid? RegistrationId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, если согласие выдаётся существующим пользователем
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Тип выдаваемого согласия
    /// </summary>
    public ConsentType Type { get; init; }

    /// <summary>
    /// Идентификатор версии документа, с которой согласился пользователь
    /// </summary>
    public Guid ConsentVersionId { get; init; }

    /// <summary>
    /// Время фактической подачи согласия пользователем
    /// </summary>
    public DateTime GivenAt { get; init; }
    
    /// <summary>
    /// Время истечения временного согласия
    /// </summary>
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    /// IP-адрес клиента в момент подачи согласия
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// User-Agent клиента в момент подачи согласия
    /// </summary>
    public string? UserAgent { get; init; }
}