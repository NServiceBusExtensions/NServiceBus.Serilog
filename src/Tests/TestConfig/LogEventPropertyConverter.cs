using System;
using Newtonsoft.Json;
using Serilog.Events;

class LogEventPropertyConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var property = (LogEventProperty) value;
        writer.WriteStartObject();
        writer.WritePropertyName(property.Name);
        serializer.Serialize(writer, property.Value);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type type, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type type)
    {
        return typeof(LogEventProperty).IsAssignableFrom(type);
    }
}