using Newtonsoft.Json;

static class SerializerBuilder
{
    public static JsonSerializerSettings Get()
    {
        var jsonSerializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            SerializationBinder = new ShortNameBinder(),
            ContractResolver = new CustomContractResolver()
        };
        var guidScrubbingConverter = new GuidScrubbingConverter();
        jsonSerializerSettings.Converters.Add(guidScrubbingConverter);
        var dateTimeOffsetScrubbingConverter = new DateTimeScrubbingConverter();
        jsonSerializerSettings.Converters.Add(dateTimeOffsetScrubbingConverter);
        jsonSerializerSettings.Converters.Add(new StringScrubbingConverter(guidScrubbingConverter, dateTimeOffsetScrubbingConverter));
        return jsonSerializerSettings;
    }
}