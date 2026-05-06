using System.Text.Json;
using System.Text.Json.Serialization;

namespace EasyWeChatWeb.JsonConverters;

/// <summary>
/// 空字符串转 null 的 JSON Converter
/// 用于处理前端传递的空字符串，自动转换为 null
/// 支持 Guid?、string?、int? 等所有 nullable 类型
/// </summary>
public class EmptyStringToNullConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串且为空，返回 null
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            return value;
        }

        // 如果已经是 null，直接返回
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 string");
    }

    public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value);
        }
    }
}

/// <summary>
/// 可空 DateTime 的 JSON Converter
/// 支持多种日期格式：
/// - YYYY-MM-DD HH:mm:ss
/// - YYYY-MM-DD
/// - ISO 8601 格式
/// </summary>
public class FlexibleDateTimeConverter : JsonConverter<DateTime?>
{
    private static readonly string[] DateFormats = new[]
    {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy/MM/dd",
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy",
        "o", // ISO 8601
        "s", // Sortable format
    };

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            // 空字符串返回 null
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            // 尝试多种格式解析
            foreach (var format in DateFormats)
            {
                if (DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out var dateValue))
                {
                    return dateValue;
                }
            }

            // 最后尝试自动解析
            if (DateTime.TryParse(value, out var autoDateValue))
            {
                return autoDateValue;
            }

            throw new JsonException($"无法将 '{value}' 转换为 DateTime，支持的格式：YYYY-MM-DD HH:mm:ss、YYYY-MM-DD、ISO 8601");
        }

        // 如果已经是 null，直接返回
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 DateTime?");
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            // 输出为 ISO 8601 格式
            writer.WriteStringValue(value.Value.ToString("o"));
        }
    }
}

/// <summary>
/// 非空 DateTime 的 JSON Converter
/// 支持多种日期格式
/// </summary>
public class FlexibleDateTimeConverterForNonNullable : JsonConverter<DateTime>
{
    private static readonly string[] DateFormats = new[]
    {
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd HH:mm",
        "yyyy-MM-dd",
        "yyyy/MM/dd HH:mm:ss",
        "yyyy/MM/dd",
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy",
        "o", // ISO 8601
        "s", // Sortable format
    };

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                // 非空类型收到空字符串，返回默认值
                return DateTime.MinValue;
            }

            // 尝试多种格式解析
            foreach (var format in DateFormats)
            {
                if (DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out var dateValue))
                {
                    return dateValue;
                }
            }

            // 最后尝试自动解析
            if (DateTime.TryParse(value, out var autoDateValue))
            {
                return autoDateValue;
            }

            throw new JsonException($"无法将 '{value}' 转换为 DateTime，支持的格式：YYYY-MM-DD HH:mm:ss、YYYY-MM-DD、ISO 8601");
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // 输出为 ISO 8601 格式
        writer.WriteStringValue(value.ToString("o"));
    }
}

/// <summary>
/// Guid 空字符串转 null 的 JSON Converter
/// </summary>
public class EmptyStringToNullableGuidConverter : JsonConverter<Guid?>
{
    public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            // 空字符串返回 null
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            // 尝试解析 Guid
            if (Guid.TryParse(value, out var guid))
            {
                return guid;
            }
            throw new JsonException($"无法将 '{value}' 转换为 Guid");
        }

        // 如果已经是 null，直接返回
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 Guid?");
    }

    public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.Value.ToString());
        }
    }
}

/// <summary>
/// 可空整数空字符串转 null 的 JSON Converter
/// </summary>
public class EmptyStringToNullableIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            // 空字符串返回 null
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            // 尝试解析整数
            if (int.TryParse(value, out var intValue))
            {
                return intValue;
            }
            throw new JsonException($"无法将 '{value}' 转换为 int");
        }

        // 如果是数字
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt32();
        }

        // 如果已经是 null，直接返回
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 int?");
    }

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}

/// <summary>
/// 可空长整数空字符串转 null 的 JSON Converter
/// </summary>
public class EmptyStringToNullableLongConverter : JsonConverter<long?>
{
    public override long? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            // 空字符串返回 null
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            // 尝试解析长整数
            if (long.TryParse(value, out var longValue))
            {
                return longValue;
            }
            throw new JsonException($"无法将 '{value}' 转换为 long");
        }

        // 如果是数字
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetInt64();
        }

        // 如果已经是 null，直接返回
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 long?");
    }

    public override void Write(Utf8JsonWriter writer, long? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteNumberValue(value.Value);
        }
    }
}

/// <summary>
/// 可空布尔值空字符串转 null 的 JSON Converter
/// </summary>
public class EmptyStringToNullableBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // 如果是字符串
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            // 空字符串返回 null
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }
            // 尝试解析布尔值
            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }
            throw new JsonException($"无法将 '{value}' 转换为 bool");
        }

        // 如果是 true/false
        if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
        {
            return reader.GetBoolean();
        }

        // 如果已经是 null，直接返回
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        // 其他类型抛出异常
        throw new JsonException($"无法将 {reader.TokenType} 转换为 bool?");
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteBooleanValue(value.Value);
        }
    }
}