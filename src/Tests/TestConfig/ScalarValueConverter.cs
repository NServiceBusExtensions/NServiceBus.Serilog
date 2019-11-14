using System;
using Newtonsoft.Json;
using Serilog.Events;

class ScalarValueConverter :
    JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        var property = (ScalarValue) value!;
        serializer.Serialize(writer, property.Value);
    }

    public override object ReadJson(JsonReader reader, Type type, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type type)
    {
        return typeof(ScalarValue).IsAssignableFrom(type);
    }
}