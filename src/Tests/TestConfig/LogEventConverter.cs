using Newtonsoft.Json;

class LogEventConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var logEvent = (LogEventEx) value!;
        writer.WriteStartObject();
        writer.WritePropertyName("MessageTemplate");
        serializer.Serialize(writer, logEvent.MessageTemplate.Text);
        writer.WritePropertyName("Level");
        serializer.Serialize(writer, logEvent.Level);
        writer.WritePropertyName("Properties");
        serializer.Serialize(writer, logEvent.Properties);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer) =>
        throw new();

    public override bool CanConvert(Type type) =>
        typeof(LogEventEx).IsAssignableFrom(type);
}