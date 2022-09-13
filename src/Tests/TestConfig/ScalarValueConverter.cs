class ScalarValueConverter :
    WriteOnlyJsonConverter<ScalarValue>
{
    public override void Write(VerifyJsonWriter writer, ScalarValue value)
    {
        if (value.Value != null)
        {
            writer.Serialize(value.Value);
        }
    }
}