using Planara.Common.Enums;

namespace Planara.Common.Kafka.Messages.Privacy;

/// <summary>
/// Событие успешной фиксации согласия пользователя
/// </summary>
public class ConsentGrantedMessage
{
    /// <summary>
    /// Идентификатор созданного согласия
    /// </summary>
    public Guid ConsentId { get; init; }

    /// <summary>
    /// Идентификатор исходного запроса на выдачу согласия
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Идентификатор регистрационной сессии, в рамках которой было выдано согласие
    /// </summary>
    public Guid? RegistrationId { get; init; }

    /// <summary>
    /// Идентификатор пользователя, которому принадлежит согласие
    /// </summary>
    public Guid? UserId { get; init; }

    /// <summary>
    /// Тип выданного согласия
    /// </summary>
    public ConsentType Type { get; init; }

    /// <summary>
    /// Идентификатор версии документа, на которую было выдано согласие
    /// </summary>
    public Guid ConsentVersionId { get; init; }

    /// <summary>
    /// Время фактической подачи согласия пользователем
    /// </summary>
    public DateTime GivenAt { get; init; }
}