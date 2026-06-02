namespace Planara.Common.Kafka;

/// <summary>
/// Ключи Kafka-топиков
/// </summary>
public static class KafkaTopicKeys
{
    /// <summary>
    /// Событие создания пользователя
    /// </summary>
    public const string UserCreated = "UserCreated";

    /// <summary>
    /// Событие удаления пользователя
    /// </summary>
    public const string UserDeleted = "UserDeleted";
}