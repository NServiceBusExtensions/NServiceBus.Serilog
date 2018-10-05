using System;
using Newtonsoft.Json;

class DateTimeOffsetScrubbingConverter : JsonConverter
{
    int count;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        count++;
        writer.WriteValue($"DateTimeOffset {count}");
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new Exception();
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(DateTimeOffset);
    }
}