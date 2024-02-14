using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Toxiproxy.Net
{
    /// <summary>
    /// A custom StringEnumConverter that serializes enum values as lowercase strings.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Converters.StringEnumConverter"/>
    public class LowercaseStringEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Enum)
            {
                writer.WriteValue(value.ToString().ToLowerInvariant());
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }
    }
}