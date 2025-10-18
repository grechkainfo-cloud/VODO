using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace Vodo.Server.SystemTextJsonConverters
{
    public class GeometrySystemTextJsonConverter : JsonConverter<Geometry>
    {
        private readonly GeoJsonWriter _writer = new();
        private readonly GeoJsonReader _reader = new();

        public override Geometry? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var json = doc.RootElement.GetRawText();
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return _reader.Read<Geometry>(json);
        }

        public override void Write(Utf8JsonWriter writer, Geometry value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            // GeoJsonWriter.Write returns a JSON string; используем WriteRawValue чтобы вставить валидный JSON
            var geoJson = _writer.Write(value);
            writer.WriteRawValue(geoJson);
        }
    }
}
