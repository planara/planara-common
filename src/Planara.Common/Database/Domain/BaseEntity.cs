namespace Planara.Common.Database.Domain;

/// <summary>
/// Базовая сущность
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// ID сущности
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Дата создания сущности
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Дата обновления сущности
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}