using Planara.Common.Enums;

namespace Planara.Common.Kafka.Messages.Privacy;

/// <summary>
/// Событие отзыва ранее выданного согласия пользователя.
/// </summary>
public class ConsentRevokedMessage
{
    /// <summary>
    /// Идентификатор отозванного согласия
    /// </summary>
    public Guid ConsentId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, отозвавшего согласие
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// Тип отозванного согласия
    /// </summary>
    public ConsentType Type { get; init; }

    /// <summary>
    /// Идентификатор версии документа, согласие на которую было отозвано
    /// </summary>
    public Guid ConsentVersionId { get; init; }

    /// <summary>
    /// Время отзыва согласия
    /// </summary>
    public DateTime RevokedAt { get; init; }
}