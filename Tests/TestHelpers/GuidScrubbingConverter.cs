using System;
using Newtonsoft.Json;

class GuidScrubbingConverter : JsonConverter
{
    int count;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        WriteValue(writer);
    }

    public void WriteValue(JsonWriter writer)
    {
        count++;
        writer.WriteValue($"Guid {count}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new Exception();
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type type)
    {
        return type == typeof(Guid);
    }
}