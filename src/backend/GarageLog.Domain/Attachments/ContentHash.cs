using GarageLog.Domain.Infrastructure;
using GarageLog.SharedKernel;
using static GarageLog.Domain.Errors.ContentHashErrors;

namespace GarageLog.Domain.Attachments;

/// <summary>
/// SHA-256 хеш содержимого файла (hex, lowercase, 64 символа).
/// Используется для дедупликации и проверки целостности.
/// </summary>
public class ContentHash : ValueObject
{
    /// <summary>Длина hex-представления SHA-256.</summary>
    public const int HexLength = 64;

    /// <summary>Hex-представление хеша (lowercase).</summary>
    public string Value { get; }

    private ContentHash(string value) => Value = value;

    /// <summary>
    /// Создаёт хеш из hex-строки.
    /// </summary>
    /// <param name="input">Hex-строка (регистр не важен).</param>
    /// <exception cref="DomainException">Если строка не является валидным SHA-256 hex.</exception>
    public static ContentHash From(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new DomainException<ContentHash>(InputStringIsEmpty);
        var normalized = input.Trim().ToLowerInvariant();

        if (normalized.Length != HexLength)
            throw new DomainException<ContentHash>(InvalidLength);

        if (!normalized.All(IsHexChar))
            throw new DomainException<ContentHash>(InvalidChars);

        return new ContentHash(normalized);
    }

    /// <summary>
    /// Создаёт хеш из массива байт (вычисляет SHA-256).
    /// </summary>
    /// <param name="bytes">Содержимое файла.</param>
    public static ContentHash FromBytes(byte[] bytes)
    {
        if (bytes is null || bytes.Length == 0)
            throw new DomainException<ContentHash>(InputBytesEmpty);

        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return new ContentHash(Convert.ToHexString(hash).ToLowerInvariant());
    }

    private static bool IsHexChar(char c) => c is >= '0' and <= '9' or >= 'a' and <= 'f';

    /// <inheritdoc/>
    public override string ToString() => Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
