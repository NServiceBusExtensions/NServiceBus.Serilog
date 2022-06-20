public class PropertyEnricherConverter :
    WriteOnlyJsonConverter<PropertyEnricher>
{
    static FieldInfo nameField;
    static FieldInfo valueField;

    static PropertyEnricherConverter()
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        nameField = typeof(PropertyEnricher).GetField("_name", flags)!;
        valueField = typeof(PropertyEnricher).GetField("_value", flags)!;
    }

    public override void Write(VerifyJsonWriter writer, PropertyEnricher enricher)
    {
        writer.WriteStartObject();
        var name = (string) nameField.GetValue(enricher)!;
        var value = valueField.GetValue(enricher)!;
        writer.WritePropertyName(name);
        writer.Serialize(value);
        writer.WriteEndObject();
    }
}