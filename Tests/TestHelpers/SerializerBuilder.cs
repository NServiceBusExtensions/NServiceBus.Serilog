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
        jsonSerializerSettings.Converters.Add(new GuidScrubbingConverter());
        jsonSerializerSettings.Converters.Add(new DateTimeOffsetScrubbingConverter());
        return jsonSerializerSettings;
    }
}