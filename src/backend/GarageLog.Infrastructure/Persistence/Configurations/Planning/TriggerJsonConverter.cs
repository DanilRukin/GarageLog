using GarageLog.Domain.Planning.Triggers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace GarageLog.Infrastructure.Persistence.Configurations.Planning;

/// <summary>
/// Конвертер <see cref="Trigger"/> ↔ JSON-строка для EF Core.
/// </summary>
public sealed class TriggerJsonConverter : ValueConverter<Trigger, string>
{
    /// <summary>
    /// Создаёт конвертер с указанными настройками JSON-сериализации.
    /// </summary>
    /// <param name="options">Настройки <see cref="JsonSerializer"/> (должны включать полиморфизм).</param>
    public TriggerJsonConverter(JsonSerializerOptions options)
        : base(
            trigger => Serialize(trigger, options),
            json => Deserialize(json, options))
    {
    }

    /// <summary>
    /// Сериализует <see cref="Trigger"/> в JSON-строку.
    /// </summary>
    private static string Serialize(Trigger trigger, JsonSerializerOptions options)
        => JsonSerializer.Serialize(trigger, options);

    /// <summary>
    /// Десериализует JSON-строку в <see cref="Trigger"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если строка не может быть десериализована.</exception>
    private static Trigger Deserialize(string json, JsonSerializerOptions options)
    {
        var result = JsonSerializer.Deserialize<Trigger>(json, options);
        if (result is null)
            throw new InvalidOperationException("Не удалось десериализовать Trigger: результат null.");
        return result;
    }
}
