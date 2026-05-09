using System.Text.Json;
using System.Text.Json.Serialization;
using ProductCatalog.Domain.Products;

namespace ProductCatalog.Api.Serialization;

/// <summary>Serializes Sku value objects as plain strings in JSON responses.</summary>
public sealed class SkuJsonConverter : JsonConverter<Sku>
{
    public override Sku Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString()
            ?? throw new JsonException("SKU value cannot be null.");
        return Sku.Create(value);
    }

    public override void Write(Utf8JsonWriter writer, Sku value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}
