using Planara.Common.Enums;

namespace Planara.Common.Database.Domain;

/// <summary>
/// Локальная проекция текущего состояния согласия пользователя
/// </summary>
public sealed class UserConsentProjection
{
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Тип согласия
    /// </summary>
    public ConsentType Type { get; set; }

    /// <summary>
    /// Идентификатор версии согласия.
    /// </summary>
    public Guid ConsentVersionId { get; set; }

    /// <summary>
    /// Признак наличия действующего согласия
    /// </summary>
    public bool IsGranted { get; set; }

    /// <summary>
    /// Время первоначальной выдачи согласия
    /// </summary>
    public DateTime GrantedAt { get; set; }

    /// <summary>
    /// Время отзыва согласия
    /// </summary>
    public DateTime? RevokedAt { get; set; }

    /// <summary>
    /// Время последнего изменения проекции
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}