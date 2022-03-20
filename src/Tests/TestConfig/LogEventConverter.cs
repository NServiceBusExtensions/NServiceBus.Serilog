class LogEventConverter :
    WriteOnlyJsonConverter<LogEventEx>
{
    public override void Write(VerifyJsonWriter writer, LogEventEx logEvent)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("MessageTemplate");
        writer.Serialize(logEvent.MessageTemplate.Text);
        writer.WritePropertyName("Level");
        writer.Serialize(logEvent.Level);
        writer.WritePropertyName("Properties");
        writer.Serialize(logEvent.Properties);
        writer.WriteEndObject();
    }
}