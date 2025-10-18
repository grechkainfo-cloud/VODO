using System;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

namespace Vodo.Server.NewtonsoftConverters
{
    public class GeometryNewtonsoftConverter : JsonConverter<Geometry>
    {
        private static readonly JsonSerializer _geoJsonSerializer = GeoJsonSerializer.Create();

        public override void WriteJson(JsonWriter writer, Geometry? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            _geoJsonSerializer.Serialize(writer, value);
        }

        public override Geometry? ReadJson(JsonReader reader, Type objectType, Geometry? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return _geoJsonSerializer.Deserialize<Geometry>(reader);
        }
    }
}
