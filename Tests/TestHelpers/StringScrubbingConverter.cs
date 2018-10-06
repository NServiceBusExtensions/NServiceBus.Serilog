using System;
using System.Globalization;
using Newtonsoft.Json;

class StringScrubbingConverter : JsonConverter
{
    GuidScrubbingConverter guidScrubbingConverter;
    DateTimeScrubbingConverter dateTimeScrubbingConverter;

    public StringScrubbingConverter(GuidScrubbingConverter guidScrubbingConverter, DateTimeScrubbingConverter dateTimeScrubbingConverter)
    {
        this.guidScrubbingConverter = guidScrubbingConverter;
        this.dateTimeScrubbingConverter = dateTimeScrubbingConverter;
    }
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var valueAsString = (string) value;
        if (!string.IsNullOrWhiteSpace(valueAsString))
        {
            if (Guid.TryParse(valueAsString, out _))
            {
                guidScrubbingConverter.WriteValue(writer);
                return;
            }

            if (DateTimeOffset.TryParse(valueAsString, out _))
            {
                dateTimeScrubbingConverter.WriteValue(writer);
                return;
            }

            if (DateTime.TryParse(valueAsString, out _))
            {
                dateTimeScrubbingConverter.WriteValue(writer);
                return;
            }

            if (DateTime.TryParseExact(valueAsString, "yyyy-MM-dd HH:mm:ss:ffffff Z", null, DateTimeStyles.None, out _))
            {
                dateTimeScrubbingConverter.WriteValue(writer);
                return;
            }
        }

        writer.WriteValue(valueAsString);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new Exception();
    }

    public override bool CanRead => false;

    public override bool CanConvert(Type type)
    {
        return type == typeof(string);
    }
}