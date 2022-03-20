class ScalarValueConverter :
    WriteOnlyJsonConverter<ScalarValue>
{
    public override void Write(VerifyJsonWriter writer, ScalarValue value) =>
        writer.Serialize(value.Value);
}