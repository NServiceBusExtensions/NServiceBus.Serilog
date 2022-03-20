class LogEventPropertyConverter :
    WriteOnlyJsonConverter<LogEventProperty>
{
    public override void Write(VerifyJsonWriter writer, LogEventProperty property)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(property.Name);
        writer.Serialize(property.Value);
        writer.WriteEndObject();
    }
}