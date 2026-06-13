using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using GreenStar.Cartography;

namespace GreenStar;

public static class BasicDataSerializer
{
    public static bool TryDeserialize<T>(string? inputValue, [NotNullWhen(returnValue: true)] out T? nativeValue)
    {
        var type = typeof(T);

        object? result = (type, inputValue) switch
        {
            (_, null) => default(T),
            ({ Name: "Boolean" }, var value) => bool.TryParse(value, out var b) ? b : false,
            ({ Name: "String" }, var value) => value,
            ({ Name: "Int16" }, var value) => short.TryParse(value, out var i16) ? i16 : 0,
            ({ Name: "Int32" }, var value) => int.TryParse(value, out var i32) ? i32 : 0,
            ({ Name: "Int64" }, var value) => long.TryParse(value, out var i64) ? i64 : 0L,
            ({ Name: "Single" }, var value) => float.TryParse(value, out var f32) ? f32 : 0f,
            ({ Name: "Double" }, var value) => double.TryParse(value, out var f64) ? f64 : 0d,
            ({ Name: "Guid" }, var value) => Guid.TryParse(value, out var guid) ? guid : Guid.Empty,
            ({ IsEnum: true }, var value) => Enum.TryParse(type, value, out var enumValue) ? enumValue : default(T),
            ({ Name: "IEnumerable`1", IsGenericType: true, GenericTypeArguments: [Type { Name: "Guid" }] }, var value) => value.Split(",").Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty).ToArray(),
            ({ IsArray: true }, var value) when type.GetElementType() == typeof(Guid) => value.Split(",").Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty).ToArray(),
            ({ Name: nameof(Coordinate) }, var value) => (Coordinate)value,
            ({ Name: nameof(Vector) }, var value) => (Vector)value,
            _ => throw new InvalidOperationException($"not supported type conversion: {type.Name}"),
        };

        nativeValue = (T?)result;

        return inputValue is not null && result is not null;
    }

    public static string? SerializeToString(object? value)
        => value switch
        {
            null => null,
            bool b => Convert.ToString(b),
            string s => s,
            short i16 => Convert.ToString(i16),
            int i32 => Convert.ToString(i32),
            long i64 => Convert.ToString(i64),
            float f => Convert.ToString(f),
            double d => Convert.ToString(d),
            Guid g => g.ToString(),
            Enum e => e.ToString(),
            IEnumerable<Guid> ga => string.Join(",", ga.Select(i => i.ToString())),
            Coordinate cood => cood.ToString(),
            Vector vector => vector.ToString(),
            _ => throw new NotSupportedException("not supported data type in conversion"),
        };
}